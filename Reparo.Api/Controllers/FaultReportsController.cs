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
public class FaultReportsController : ControllerBase
{
    private readonly AppDbContext _context;

    public FaultReportsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<List<FaultReportDto>>> GetAll(
        [FromQuery] FaultReportFilterDto filter)
    {
        var query = _context.FaultReports
            .Include(f => f.Assignments)
    .ThenInclude(a => a.Interventions)
        .ThenInclude(i => i.Materials)
            .ThenInclude(m => m.Material)
.Include(f => f.Assignments)
    .ThenInclude(a => a.Interventions)
        .ThenInclude(i => i.Materials)
            .ThenInclude(m => m.MaterialUnit)
            .Include(f => f.Location)
            .Include(f => f.ReportedByEmployee)
            .Include(f => f.FaultType)
            .Include(f => f.FaultPriority)
            .Include(f => f.FaultStatus)
            .Include(f => f.Assignments.Where(a => a.IsActive))
                .ThenInclude(a => a.Technician)
            .Where(f => !f.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.SearchText))
            query = query.Where(f => f.Title.Contains(filter.SearchText) ||
                                     f.Description.Contains(filter.SearchText));
        if (filter.DateFrom.HasValue)
            query = query.Where(f => f.CreatedAt >= filter.DateFrom.Value);

        if (filter.DateTo.HasValue)
            query = query.Where(f => f.CreatedAt <= filter.DateTo.Value.AddDays(1));

        if (filter.LocationId.HasValue)
            query = query.Where(f => f.LocationId == filter.LocationId);

        if (filter.FaultTypeId.HasValue)
            query = query.Where(f => f.FaultTypeId == filter.FaultTypeId);

        if (filter.FaultPriorityId.HasValue)
            query = query.Where(f => f.FaultPriorityId == filter.FaultPriorityId);

        if (filter.FaultStatusId.HasValue)
            query = query.Where(f => f.FaultStatusId == filter.FaultStatusId);

        if (filter.DateFrom.HasValue)
            query = query.Where(f => f.CreatedAt >= filter.DateFrom);

        if (filter.DateTo.HasValue)
            query = query.Where(f => f.CreatedAt <= filter.DateTo);

        query = filter.SortBy switch
        {
            "priority" => filter.SortDescending
                ? query.OrderByDescending(f => f.FaultPriorityId)
                : query.OrderBy(f => f.FaultPriorityId),
            "status" => filter.SortDescending
                ? query.OrderByDescending(f => f.FaultStatusId)
                : query.OrderBy(f => f.FaultStatusId),
            _ => filter.SortDescending
                ? query.OrderByDescending(f => f.CreatedAt)
                : query.OrderBy(f => f.CreatedAt)
        };

        var reports = await query.Select(f => new FaultReportDto
        {
            Id = f.Id,
            Title = f.Title,
            Description = f.Description,
            LocationId = f.LocationId,
            LocationName = f.Location.Name,
            ReportedByEmployeeId = f.ReportedByEmployeeId,
            ReportedByEmployeeName = f.ReportedByEmployee.FirstName
                + " " + f.ReportedByEmployee.LastName,
            FaultTypeId = f.FaultTypeId,
            FaultTypeName = f.FaultType != null ? f.FaultType.Name : null,
            FaultPriorityId = f.FaultPriorityId,
            FaultPriorityName = f.FaultPriority != null ? f.FaultPriority.Name : null,
            FaultStatusId = f.FaultStatusId,
            FaultStatusName = f.FaultStatus.Name,
            Deadline = f.Deadline,
            CreatedAt = f.CreatedAt,
            IsDeleted = f.IsDeleted,
            HasActiveFailedIntervention = f.Assignments
            .Where(a => a.IsActive)
            .SelectMany(a => a.Interventions)
            .Any(i => i.InterventionStatus.Name == "Failed"),
            ActiveTechnicianName = f.Assignments
                .Where(a => a.IsActive)
                .Select(a => a.Technician.FirstName + " " + a.Technician.LastName)
                .FirstOrDefault(),
            AllMaterials = f.Assignments
                .SelectMany(a => a.Interventions)
                .SelectMany(i => i.Materials)
                .Select(m => new InterventionMaterialDto
                {
                    MaterialName = m.Material.Name,
                    Quantity = m.Quantity,
                    MaterialUnitName = m.MaterialUnit.Name
                }).ToList(),
        }).ToListAsync();

        return Ok(reports);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<FaultReportDto>> GetById(int id)
    {
        var f = await _context.FaultReports
            .Include(f => f.Assignments)
    .ThenInclude(a => a.Interventions)
        .ThenInclude(i => i.Materials)
            .ThenInclude(m => m.Material)
.Include(f => f.Assignments)
    .ThenInclude(a => a.Interventions)
        .ThenInclude(i => i.Materials)
            .ThenInclude(m => m.MaterialUnit)
            .Include(f => f.Location)
            .Include(f => f.ReportedByEmployee)
            .Include(f => f.FaultType)
            .Include(f => f.FaultPriority)
            .Include(f => f.FaultStatus)
            .Include(f => f.Assignments.Where(a => a.IsActive))
                .ThenInclude(a => a.Technician)
            .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);

        if (f is null)
            return NotFound();

        return Ok(new FaultReportDto
        {
            Id = f.Id,
            Title = f.Title,
            Description = f.Description,
            LocationId = f.LocationId,
            LocationName = f.Location.Name,
            ReportedByEmployeeId = f.ReportedByEmployeeId,
            ReportedByEmployeeName = f.ReportedByEmployee.FirstName
                + " " + f.ReportedByEmployee.LastName,
            FaultTypeId = f.FaultTypeId,
            FaultTypeName = f.FaultType?.Name,
            FaultPriorityId = f.FaultPriorityId,
            FaultPriorityName = f.FaultPriority?.Name,
            FaultStatusId = f.FaultStatusId,
            FaultStatusName = f.FaultStatus.Name,
            Deadline = f.Deadline,
            CreatedAt = f.CreatedAt,
            ActiveTechnicianName = f.Assignments
                .Where(a => a.IsActive)
                .Select(a => a.Technician.FirstName + " " + a.Technician.LastName)
                .FirstOrDefault(),
            AiSummary = f.AiSummary,
            AiSummaryGeneratedAt = f.AiSummaryGeneratedAt,
            AllMaterials = f.Assignments
            .SelectMany(a => a.Interventions)
            .SelectMany(i => i.Materials)
            .Select(m => new InterventionMaterialDto
            {
                MaterialName = m.Material.Name,
                Quantity = m.Quantity,
                MaterialUnitName = m.MaterialUnit.Name
            }).ToList(),
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager,Reporter")]
    public async Task<ActionResult<FaultReportDto>> Create(FaultReportCreateDto dto)
    {
        var location = await _context.Locations.FindAsync(dto.LocationId);
        if (location is null || !location.IsActive)
            return BadRequest("Location does not exist or is not active.");

        var statusZaprimljeno = await _context.FaultStatuses
            .FirstOrDefaultAsync(s => s.Name == "Submitted");

        if (statusZaprimljeno is null)
            return StatusCode(500, "Status Submitted not found.");

        var report = new FaultReport
        {
            Title = dto.Title,
            Description = dto.Description,
            LocationId = dto.LocationId,
            ReportedByEmployeeId = dto.ReportedByEmployeeId,
            FaultStatusId = statusZaprimljeno.Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.FaultReports.Add(report);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = report.Id },
            new FaultReportDto
            {
                Id = report.Id,
                Title = report.Title,
                Description = report.Description,
                LocationId = report.LocationId,
                FaultStatusId = report.FaultStatusId,
                FaultStatusName = statusZaprimljeno.Name,
                CreatedAt = report.CreatedAt
            });
    }

    [HttpPut("{id}/review")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult> Review(int id, FaultReportUpdateDto dto)
    {
        var report = await _context.FaultReports.FindAsync(id);
        if (report is null || report.IsDeleted)
            return NotFound();

        if (dto.FaultPriorityId.HasValue)
        {
            var priority = await _context.FaultPriorities
                .FirstOrDefaultAsync(p => p.Id == dto.FaultPriorityId);

            if (priority?.Name == "Critical" && dto.Deadline is null)
                return BadRequest("Critical priority requires a deadline.");
        }

        report.FaultTypeId = dto.FaultTypeId;
        report.FaultPriorityId = dto.FaultPriorityId;
        report.Deadline = dto.Deadline;

        var statusReviewed = await _context.FaultStatuses
            .FirstOrDefaultAsync(s => s.Name == "Reviewed");

        if (statusReviewed is not null)
            report.FaultStatusId = statusReviewed.Id;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id}/close")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult> Close(int id)
    {
        var report = await _context.FaultReports
            .Include(f => f.Assignments)
                .ThenInclude(a => a.Interventions)
                    .ThenInclude(i => i.InterventionStatus)
            .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);

        if (report is null)
            return NotFound();

        var hasSuccessful = report.Assignments
            .SelectMany(a => a.Interventions)
            .Any(i => i.InterventionStatus.Name == "Completed");

        if (!hasSuccessful)
            return BadRequest("The report cannot be closed without a successfully completed intervention.");

        var statusClosed = await _context.FaultStatuses
            .FirstOrDefaultAsync(s => s.Name == "Closed");

        if (statusClosed is not null)
            report.FaultStatusId = statusClosed.Id;

        report.ClosedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("mine")]
    [Authorize(Roles = "Reporter,Admin")]
    public async Task<ActionResult<List<FaultReportDto>>> GetMine()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.EmployeeId is null)
            return BadRequest("Korisnik nije povezan s djelatnikom.");

        var reports = await _context.FaultReports
            .Include(f => f.Location)
            .Include(f => f.FaultType)
            .Include(f => f.FaultPriority)
            .Include(f => f.FaultStatus)
            .Include(f => f.Assignments.Where(a => a.IsActive))
                .ThenInclude(a => a.Technician)
            .Where(f => f.ReportedByEmployeeId == user.EmployeeId && !f.IsDeleted)
            .Select(f => new FaultReportDto
            {
                Id = f.Id,
                Title = f.Title,
                Description = f.Description,
                LocationId = f.LocationId,
                LocationName = f.Location.Name,
                FaultTypeName = f.FaultType != null ? f.FaultType.Name : null,
                FaultPriorityName = f.FaultPriority != null ? f.FaultPriority.Name : null,
                FaultStatusId = f.FaultStatusId,
                FaultStatusName = f.FaultStatus.Name,
                Deadline = f.Deadline,
                CreatedAt = f.CreatedAt,
                ActiveTechnicianName = f.Assignments
                    .Where(a => a.IsActive)
                    .Select(a => a.Technician.FirstName + " " + a.Technician.LastName)
                    .FirstOrDefault()
            })
            .ToListAsync();

        return Ok(reports);
    }

    [HttpGet("{id}/timeline")]
    public async Task<ActionResult<List<TimelineEventDto>>> GetTimeline(int id)
    {
        var report = await _context.FaultReports
            .Include(f => f.FaultStatus)
            .Include(f => f.ReportedByEmployee)
            .Include(f => f.Assignments)
                .ThenInclude(a => a.Technician)
            .Include(f => f.Assignments)
                .ThenInclude(a => a.Interventions)
                    .ThenInclude(i => i.InterventionStatus)
            .Include(f => f.Attachments)
            .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);

        if (report is null)
            return NotFound();

        var events = new List<TimelineEventDto>();

        if (report.FaultStatus.Name == "Closed" && report.ClosedAt.HasValue)
        {
            events.Add(new TimelineEventDto
            {
                Timestamp = report.ClosedAt.Value,
                EventType = "Closed",
                Description = "Report closed by manager.",
                Actor = "Manager",
                Color = "success",
                Icon = "CheckCircle"
            });
        }

        foreach (var att in report.Attachments.OrderBy(a => a.UploadedAt))
        {
            events.Add(new TimelineEventDto
            {
                Timestamp = att.UploadedAt,
                EventType = "Attachment",
                Description = $"{att.Purpose} uploaded: {att.OriginalFileName}",
                Actor = "System",
                Color = "info",
                Icon = "AttachFile"
            });
        }

        foreach (var assignment in report.Assignments.OrderBy(a => a.AssignedAt))
        {
            events.Add(new TimelineEventDto
            {
                Timestamp = assignment.AssignedAt,
                EventType = "Assigned",
                Description = $"Assigned to {assignment.Technician.FirstName} " +
                              $"{assignment.Technician.LastName}" +
                              (assignment.Note != null ? $" — {assignment.Note}" : ""),
                Actor = "Manager",
                Color = assignment.IsActive ? "secondary" : "default",
                Icon = "Assignment"
            });

            foreach (var intervention in assignment.Interventions.OrderBy(i => i.CreatedAt))
            {
                events.Add(new TimelineEventDto
                {
                    Timestamp = intervention.CreatedAt,
                    EventType = "InterventionStarted",
                    Description = "Intervention started",
                    Actor = assignment.Technician.FirstName + " " +
                            assignment.Technician.LastName,
                    Color = "warning",
                    Icon = "Build"
                });

                if (intervention.FinishedAt.HasValue)
                {
                    var isCompleted = intervention.InterventionStatus.Name == "Completed";
                    events.Add(new TimelineEventDto
                    {
                        Timestamp = intervention.FinishedAt.Value,
                        EventType = isCompleted ? "InterventionCompleted" : "InterventionFailed",
                        Description = $"Intervention {intervention.InterventionStatus.Name.ToLower()}" +
                                      (intervention.Note != null && intervention.Note != "-"
                                          ? $": {intervention.Note}" : ""),
                        Actor = assignment.Technician.FirstName + " " +
                                assignment.Technician.LastName,
                        Color = isCompleted ? "success" : "error",
                        Icon = isCompleted ? "CheckCircle" : "Cancel"
                    });
                }
            }
        }
        return Ok(events.OrderBy(e => e.Timestamp).ToList());
    }

    [HttpPut("{id}/edit")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult> Edit(int id, FaultReportCreateDto dto)
    {
        var report = await _context.FaultReports.FindAsync(id);

        if (report is null || report.IsDeleted)
            return NotFound();

        report.Title = dto.Title;
        report.Description = dto.Description;

        await _context.SaveChangesAsync();
        return NoContent();
    }
}