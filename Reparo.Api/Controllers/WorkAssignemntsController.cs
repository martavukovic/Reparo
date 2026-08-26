using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reparo.Api.Data;
using Reparo.Shared.DTOs;
using Reparo.Shared.Models;

namespace Reparo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkAssignmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public WorkAssignmentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<List<WorkAssignmentDto>>> GetAll()
    {
        var assignments = await _context.WorkAssignments
            .Include(a => a.FaultReport)
                .ThenInclude(f => f.Location)
            .Include(a => a.FaultReport)
                .ThenInclude(f => f.FaultType)
            .Include(a => a.FaultReport)
                .ThenInclude(f => f.FaultPriority)
            .Include(a => a.FaultReport)
                .ThenInclude(f => f.FaultStatus)
            .Include(a => a.Technician)
            .Include(a => a.Interventions)
                .ThenInclude(i => i.InterventionStatus)
            .Select(a => new WorkAssignmentDto
            {
                Id = a.Id,
                FaultReportId = a.FaultReportId,
                FaultReportTitle = a.FaultReport.Title,
                LocationName = a.FaultReport.Location.Name,
                FaultTypeName = a.FaultReport.FaultType != null
                    ? a.FaultReport.FaultType.Name : "",
                FaultPriorityName = a.FaultReport.FaultPriority != null
                    ? a.FaultReport.FaultPriority.Name : "",
                FaultStatusName = a.FaultReport.FaultStatus.Name,
                TechnicianId = a.TechnicianId,
                TechnicianName = a.Technician.FirstName + " " + a.Technician.LastName,
                AssignedAt = a.AssignedAt,
                IsActive = a.IsActive,
                Note = a.Note
            })
            .ToListAsync();

        return Ok(assignments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkAssignmentDto>> GetById(int id)
    {
        var a = await _context.WorkAssignments
            .Include(a => a.FaultReport)
                .ThenInclude(f => f.Location)
            .Include(a => a.FaultReport)
                .ThenInclude(f => f.FaultType)
            .Include(a => a.FaultReport)
                .ThenInclude(f => f.FaultPriority)
            .Include(a => a.FaultReport)
                .ThenInclude(f => f.FaultStatus)
            .Include(a => a.Technician)
            .Include(a => a.Interventions)
                .ThenInclude(i => i.InterventionStatus)
            .Include(a => a.Interventions)
                .ThenInclude(i => i.Materials)
                    .ThenInclude(m => m.Material)
            .Include(a => a.Interventions)
                .ThenInclude(i => i.Materials)
                    .ThenInclude(m => m.MaterialUnit)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (a is null)
            return NotFound();

        return Ok(new WorkAssignmentDto
        {
            Id = a.Id,
            FaultReportId = a.FaultReportId,
            FaultReportTitle = a.FaultReport.Title,
            LocationName = a.FaultReport.Location.Name,
            FaultTypeName = a.FaultReport.FaultType?.Name ?? "",
            FaultPriorityName = a.FaultReport.FaultPriority?.Name ?? "",
            FaultStatusName = a.FaultReport.FaultStatus.Name,
            TechnicianId = a.TechnicianId,
            TechnicianName = a.Technician.FirstName + " " + a.Technician.LastName,
            AssignedAt = a.AssignedAt,
            IsActive = a.IsActive,
            Note = a.Note,
            Interventions = a.Interventions.Select(i => new InterventionDto
            {
                Id = i.Id,
                WorkAssignmentId = i.WorkAssignmentId,
                InterventionStatusId = i.InterventionStatusId,
                InterventionStatusName = i.InterventionStatus.Name,
                StartedAt = i.StartedAt,
                FinishedAt = i.FinishedAt,
                DurationMinutes = i.DurationMinutes,
                Note = i.Note,
                CreatedAt = i.CreatedAt,
                Materials = i.Materials.Select(m => new InterventionMaterialDto
                {
                    Id = m.Id,
                    MaterialId = m.MaterialId,
                    MaterialName = m.Material.Name,
                    Quantity = m.Quantity,
                    MaterialUnitId = m.MaterialUnitId,
                    MaterialUnitName = m.MaterialUnit.Name
                }).ToList()
            }).ToList()
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<WorkAssignmentDto>> Assign(WorkAssignmentCreateDto dto)
    {
        var report = await _context.FaultReports
            .Include(f => f.Assignments)
            .Include(f => f.FaultStatus)
            .FirstOrDefaultAsync(f => f.Id == dto.FaultReportId && !f.IsDeleted);

        if (report is null)
            return NotFound("Report not found.");

        if (report.FaultStatus?.Name == "Resolved" || report.FaultStatus?.Name == "Closed")
            return BadRequest("Cannot assign a resolved or closed fault report.");

        var technician = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == dto.TechnicianId
                && e.IsTechnician && e.IsActive);

        if (technician is null)
            return BadRequest("Technician not found or is not active.");

        // Deactivate existing assignment
        var activeAssignment = report.Assignments
            .FirstOrDefault(a => a.IsActive);

        if (activeAssignment is not null)
            activeAssignment.IsActive = false;

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

        var assignment = new WorkAssignment
        {
            FaultReportId = dto.FaultReportId,
            TechnicianId = dto.TechnicianId,
            AssignedAt = DateTime.UtcNow,
            AssignedByUserId = userId,
            IsActive = true,
            Note = dto.Note
        };

        _context.WorkAssignments.Add(assignment);


        var statusAssigned = await _context.FaultStatuses
            .FirstOrDefaultAsync(s => s.Name == "Assigned");

        if (statusAssigned is not null)
            report.FaultStatusId = statusAssigned.Id;

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = assignment.Id },
            new WorkAssignmentDto
            {
                Id = assignment.Id,
                FaultReportId = assignment.FaultReportId,
                TechnicianId = assignment.TechnicianId,
                TechnicianName = technician.FirstName + " " + technician.LastName,
                AssignedAt = assignment.AssignedAt,
                IsActive = assignment.IsActive,
                Note = assignment.Note
            });
    }

    [HttpGet("mine")]
    [Authorize(Roles = "Technician,Admin")]
    public async Task<ActionResult<List<WorkAssignmentDto>>> GetMine()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.EmployeeId is null)
            return BadRequest("Korisnik nije povezan s izvršiteljem.");

        var assignments = await _context.WorkAssignments
            .Include(a => a.FaultReport)
                .ThenInclude(f => f.Location)
            .Include(a => a.FaultReport)
                .ThenInclude(f => f.FaultType)
            .Include(a => a.FaultReport)
                .ThenInclude(f => f.FaultPriority)
            .Include(a => a.FaultReport)
                .ThenInclude(f => f.FaultStatus)
            .Include(a => a.Technician)
            .Include(a => a.Interventions)
                .ThenInclude(i => i.InterventionStatus)
            .Include(a => a.Interventions)
                .ThenInclude(i => i.Materials)
                    .ThenInclude(m => m.Material)
            .Include(a => a.Interventions)
                .ThenInclude(i => i.Materials)
                    .ThenInclude(m => m.MaterialUnit)
            .Where(a => a.TechnicianId == user.EmployeeId)
            .Select(a => new WorkAssignmentDto
            {
                Id = a.Id,
                FaultReportId = a.FaultReportId,
                FaultReportTitle = a.FaultReport.Title,
                LocationName = a.FaultReport.Location.Name,
                FaultTypeName = a.FaultReport.FaultType != null
                    ? a.FaultReport.FaultType.Name : "",
                FaultPriorityName = a.FaultReport.FaultPriority != null
                    ? a.FaultReport.FaultPriority.Name : "",
                FaultStatusName = a.FaultReport.FaultStatus.Name,
                TechnicianId = a.TechnicianId,
                TechnicianName = a.Technician.FirstName + " " + a.Technician.LastName,
                AssignedAt = a.AssignedAt,
                IsActive = a.IsActive,
                Note = a.Note,
                Interventions = a.Interventions.Select(i => new InterventionDto
                {
                    Id = i.Id,
                    WorkAssignmentId = i.WorkAssignmentId,
                    InterventionStatusId = i.InterventionStatusId,
                    InterventionStatusName = i.InterventionStatus.Name,
                    StartedAt = i.StartedAt,
                    FinishedAt = i.FinishedAt,
                    DurationMinutes = i.DurationMinutes,
                    Note = i.Note,
                    CreatedAt = i.CreatedAt,
                    Materials = i.Materials.Select(m => new InterventionMaterialDto
                    {
                        Id = m.Id,
                        MaterialId = m.MaterialId,
                        MaterialName = m.Material.Name,
                        Quantity = m.Quantity,
                        MaterialUnitId = m.MaterialUnitId,
                        MaterialUnitName = m.MaterialUnit.Name
                    }).ToList()
                }).ToList()
            })
            .ToListAsync();

        return Ok(assignments);
    }
}