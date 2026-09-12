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
        var isReporter = User.IsInRole("Reporter") && !User.IsInRole("Admin") && !User.IsInRole("Manager");


        var now = DateTime.UtcNow;

        var openReports = await _context.FaultReports
            .Include(f => f.FaultStatus)
            .Where(f => !f.IsDeleted &&
                f.FaultStatus.Name != "Closed" &&
                f.FaultStatus.Name != "Resolved")
            .ToListAsync();

        var criticalPriorityId = await _context.FaultPriorities
            .Where(p => p.Name == "Critical")
            .Select(p => p.Id)
            .FirstOrDefaultAsync();

        var closedStatusId = await _context.FaultStatuses
            .Where(s => s.Name == "Closed")
            .Select(s => s.Id)
            .FirstOrDefaultAsync();

        var closedReports = await _context.FaultReports
            .Include(f => f.Assignments)
                .ThenInclude(a => a.Interventions)
            .Where(f => f.FaultStatusId == closedStatusId && !f.IsDeleted)
            .ToListAsync();

        double avgResolution = 0;

        if (closedReports.Count > 0)
        {
            var days = closedReports
                .Select(f => f.Assignments
                    .SelectMany(a => a.Interventions)
                    .Where(i => i.FinishedAt != null)
                    .Select(i => (i.FinishedAt!.Value - f.CreatedAt).TotalDays)
                    .FirstOrDefault())
                .ToList();

            avgResolution = days.Count > 0 ? days.Average() : 0;
        }

        var lastFiveQuery = _context.FaultReports
            .Include(f => f.Location)
            .Include(f => f.FaultType)
            .Include(f => f.FaultPriority)
            .Include(f => f.FaultStatus)
            .Include(f => f.Assignments.Where(a => a.IsActive))
                .ThenInclude(a => a.Technician)
            .Where(f => !f.IsDeleted);

        if (isReporter && user?.EmployeeId is not null)
            lastFiveQuery = lastFiveQuery.Where(f =>
                f.ReportedByEmployeeId == user.EmployeeId);

        var lastFive = await lastFiveQuery
            .OrderByDescending(f => f.CreatedAt)
            .Take(5)
            .Select(f => new FaultReportDto
            {
                Id = f.Id,
                Title = f.Title,
                LocationName = f.Location.Name,
                FaultTypeName = f.FaultType != null ? f.FaultType.Name : null,
                FaultPriorityName = f.FaultPriority != null ? f.FaultPriority.Name : null,
                FaultStatusName = f.FaultStatus.Name,
                CreatedAt = f.CreatedAt,
                ActiveTechnicianName = f.Assignments
                    .Where(a => a.IsActive)
                    .Select(a => a.Technician.FirstName + " " + a.Technician.LastName)
                    .FirstOrDefault()
            })
            .ToListAsync();

        var myReportsCount = 0;
        var myAssignmentsCount = 0;

        if (user?.EmployeeId is not null)
        {
            myReportsCount = await _context.FaultReports
                .CountAsync(f => f.ReportedByEmployeeId == user.EmployeeId
                    && !f.IsDeleted);

            myAssignmentsCount = await _context.WorkAssignments
                .Include(a => a.FaultReport)
                    .ThenInclude(f => f.FaultStatus)
                .CountAsync(a => a.TechnicianId == user.EmployeeId
                    && a.IsActive
                    && a.FaultReport.FaultStatus.Name != "Resolved"
                    && a.FaultReport.FaultStatus.Name != "Closed");
        }

        return Ok(new DashboardDto
        {
            TotalOpen = openReports.Count,
            TotalCritical = openReports.Count(f => f.FaultPriorityId == criticalPriorityId),
            TotalOverdue = openReports.Count(f => f.Deadline.HasValue
                && f.Deadline < now),
            TotalUnassigned = await _context.FaultReports
                .Where(f => !f.IsDeleted &&
                    !f.Assignments.Any(a => a.IsActive))
                .CountAsync(),
            TotalActiveInterventions = await _context.Interventions
    .Include(i => i.InterventionStatus)
    .CountAsync(i => i.InterventionStatus.Name == "Planned" ||
                     i.InterventionStatus.Name == "In Progress"),
            AverageResolutionDays = Math.Round(avgResolution, 1),
            LastFiveReports = lastFive,
            MyReportsCount = myReportsCount,
            MyAssignmentsCount = myAssignmentsCount
        });
    }
}