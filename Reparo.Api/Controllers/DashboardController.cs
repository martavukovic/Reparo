using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reparo.Api.Data;
using Reparo.Shared.DTOs;

namespace Reparo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardDto>> Get()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        var isAdmin = User.IsInRole("Admin");
        var isManager = User.IsInRole("Manager");
        var isTechnician = User.IsInRole("Technician") && !isAdmin && !isManager;
        var isReporter = User.IsInRole("Reporter") && !isAdmin && !isManager;

        var closedStatuses = new[] { "Closed", "Resolved" };

        if (isReporter && user?.EmployeeId is not null)
        {
            var myReports = await _context.FaultReports
                .Include(f => f.FaultStatus)
                .Include(f => f.FaultType)
                .Include(f => f.FaultPriority)
                .Include(f => f.Location)
                .Include(f => f.Assignments.Where(a => a.IsActive))
                    .ThenInclude(a => a.Technician)
                .Where(f => !f.IsDeleted &&
                            f.ReportedByEmployeeId == user.EmployeeId)
                .ToListAsync();

            var myOpen = myReports
                .Where(f => !closedStatuses.Contains(f.FaultStatus.Name))
                .ToList();

            var lastFive = myReports
                .OrderByDescending(f => f.CreatedAt)
                .Take(5)
                .Select(f => new FaultReportDto
                {
                    Id = f.Id,
                    Title = f.Title,
                    LocationName = f.Location.Name,
                    FaultTypeName = f.FaultType?.Name,
                    FaultPriorityName = f.FaultPriority?.Name,
                    FaultStatusName = f.FaultStatus.Name,
                    CreatedAt = f.CreatedAt,
                    ActiveTechnicianName = f.Assignments
                        .Where(a => a.IsActive)
                        .Select(a => a.Technician.FirstName + " " + a.Technician.LastName)
                        .FirstOrDefault()
                })
                .ToList();

            return Ok(new DashboardDto
            {
                TotalOpen = myOpen.Count,
                TotalCritical = 0,
                TotalOverdue = 0,
                TotalUnassigned = 0,
                TotalActiveInterventions = 0,
                AverageResolutionDays = 0,
                MyReportsCount = myReports.Count,
                MyAssignmentsCount = 0,
                LastFiveReports = lastFive
            });
        }

        if (isTechnician && user?.EmployeeId is not null)
        {
            var myAssignments = await _context.WorkAssignments
                .Include(a => a.FaultReport)
                    .ThenInclude(f => f.FaultStatus)
                .Include(a => a.Interventions)
                    .ThenInclude(i => i.InterventionStatus)
                .Where(a => a.TechnicianId == user.EmployeeId && a.IsActive)
                .ToListAsync();

            var activeAssignments = myAssignments
                .Where(a => a.FaultReport.FaultStatus.Name != "Resolved" &&
                            a.FaultReport.FaultStatus.Name != "Closed")
                .Count();

            var activeInterventions = myAssignments
                .SelectMany(a => a.Interventions)
                .Count(i => i.InterventionStatus.Name == "Planned" ||
                            i.InterventionStatus.Name == "In Progress");

            return Ok(new DashboardDto
            {
                TotalOpen = 0,
                TotalCritical = 0,
                TotalOverdue = 0,
                TotalUnassigned = 0,
                TotalActiveInterventions = activeInterventions,
                AverageResolutionDays = 0,
                MyReportsCount = 0,
                MyAssignmentsCount = activeAssignments,
                LastFiveReports = new()
            });
        }

        var now = DateTime.UtcNow;

        var allReports = await _context.FaultReports
            .Include(f => f.FaultStatus)
            .Include(f => f.FaultType)
            .Include(f => f.FaultPriority)
            .Include(f => f.Location)
            .Include(f => f.Assignments.Where(a => a.IsActive))
                .ThenInclude(a => a.Technician)
            .Include(f => f.Assignments)
                .ThenInclude(a => a.Interventions)
                    .ThenInclude(i => i.InterventionStatus)
            .Where(f => !f.IsDeleted)
            .ToListAsync();

        var openReports = allReports
            .Where(f => !closedStatuses.Contains(f.FaultStatus.Name))
            .ToList();

        var totalCritical = openReports
            .Count(f => f.FaultPriority?.Name == "Critical");

        var totalOverdue = openReports
            .Count(f => f.Deadline.HasValue &&
                        f.Deadline < now);

        var totalUnassigned = openReports
            .Count(f => !f.Assignments.Any());

        var totalActiveInterventions = allReports
            .SelectMany(f => f.Assignments)
            .SelectMany(a => a.Interventions)
            .Count(i => i.InterventionStatus.Name == "Planned" ||
                        i.InterventionStatus.Name == "In Progress");

        var resolvedReports = allReports
            .Where(f => closedStatuses.Contains(f.FaultStatus.Name))
            .ToList();

        double avgDays = 0;
        if (resolvedReports.Any())
        {
            var days = resolvedReports
                .Select(f =>
                {
                    var finished = f.Assignments
                        .SelectMany(a => a.Interventions)
                        .Where(i => i.FinishedAt.HasValue &&
                                    i.InterventionStatus.Name == "Completed")
                        .Select(i => i.FinishedAt!.Value)
                        .FirstOrDefault();

                    return finished == default
                        ? 0
                        : (finished - f.CreatedAt).TotalDays;
                })
                .Where(d => d > 0)
                .ToList();

            if (days.Any())
                avgDays = Math.Round(days.Average(), 1);
        }

        var lastFiveAll = allReports
            .OrderByDescending(f => f.CreatedAt)
            .Take(5)
            .Select(f => new FaultReportDto
            {
                Id = f.Id,
                Title = f.Title,
                LocationName = f.Location.Name,
                FaultTypeName = f.FaultType?.Name,
                FaultPriorityName = f.FaultPriority?.Name,
                FaultStatusName = f.FaultStatus.Name,
                CreatedAt = f.CreatedAt,
                ActiveTechnicianName = f.Assignments
                    .Where(a => a.IsActive)
                    .Select(a => a.Technician.FirstName + " " + a.Technician.LastName)
                    .FirstOrDefault()
            })
            .ToList();

        return Ok(new DashboardDto
        {
            TotalOpen = openReports.Count,
            TotalCritical = totalCritical,
            TotalOverdue = totalOverdue,
            TotalUnassigned = totalUnassigned,
            TotalActiveInterventions = totalActiveInterventions,
            AverageResolutionDays = avgDays,
            MyReportsCount = 0,
            MyAssignmentsCount = 0,
            LastFiveReports = lastFiveAll
        });
    }
}