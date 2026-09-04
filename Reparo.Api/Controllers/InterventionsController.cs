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
public class InterventionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public InterventionsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [Authorize(Roles = "Technician,Admin")]
    public async Task<ActionResult<InterventionDto>> Create(InterventionCreateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        var assignment = await _context.WorkAssignments
            .Include(a => a.FaultReport)
            .FirstOrDefaultAsync(a => a.Id == dto.WorkAssignmentId && a.IsActive);

        if (assignment is null)
            return NotFound("Active assignment not found.");

        if (user?.EmployeeId != assignment.TechnicianId &&
            !User.IsInRole("Admin"))
            return Forbid();

        // Provjera mora biti OVDJE - prije kreiranja
        var existingActive = await _context.Interventions
            .Include(i => i.InterventionStatus)
            .Where(i => i.WorkAssignmentId == dto.WorkAssignmentId &&
                (i.InterventionStatus.Name == "Planned" ||
                 i.InterventionStatus.Name == "In Progress"))
            .FirstOrDefaultAsync();

        if (existingActive is not null)
            return BadRequest("An active intervention already exists for this assignment.");

        var statusPlanned = await _context.InterventionStatuses
            .FirstOrDefaultAsync(s => s.Name == "Planned");

        var intervention = new Intervention
        {
            WorkAssignmentId = dto.WorkAssignmentId,
            InterventionStatusId = statusPlanned!.Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.Interventions.Add(intervention);

        var statusInProgress = await _context.FaultStatuses
            .FirstOrDefaultAsync(s => s.Name == "In Progress");

        if (statusInProgress is not null)
            assignment.FaultReport.FaultStatusId = statusInProgress.Id;

        await _context.SaveChangesAsync();

        return Ok(new InterventionDto
        {
            Id = intervention.Id,
            WorkAssignmentId = intervention.WorkAssignmentId,
            InterventionStatusId = intervention.InterventionStatusId,
            InterventionStatusName = statusPlanned!.Name,
            CreatedAt = intervention.CreatedAt
        });
    }

    [HttpPut("{id}/finish")]
    [Authorize(Roles = "Technician,Admin")]
    public async Task<ActionResult> Finish(int id, InterventionFinishDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        var intervention = await _context.Interventions
            .Include(i => i.WorkAssignment)
                .ThenInclude(a => a.FaultReport)
            .Include(i => i.InterventionStatus)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (intervention is null)
            return NotFound();

        if (user?.EmployeeId != intervention.WorkAssignment.TechnicianId &&
            !User.IsInRole("Admin"))
            return Forbid();

        if (dto.StartedAt == default || dto.FinishedAt == default ||
            string.IsNullOrWhiteSpace(dto.Note))
            return BadRequest("Completed intervention must have start time, end time and a note.");
        var statusName = dto.IsSuccessful ? "Completed" : "Failed";

        var status = await _context.InterventionStatuses
            .FirstOrDefaultAsync(s => s.Name == statusName);

        intervention.StartedAt = dto.StartedAt;
        intervention.FinishedAt = dto.FinishedAt;
        intervention.Note = dto.Note;
        intervention.InterventionStatusId = status!.Id;
        intervention.DurationMinutes = (int)(dto.FinishedAt - dto.StartedAt).TotalMinutes;

        if (dto.IsSuccessful)
        {
            var statusResolved = await _context.FaultStatuses
                .FirstOrDefaultAsync(s => s.Name == "Resolved");

            if (statusResolved is not null)
                intervention.WorkAssignment.FaultReport.FaultStatusId = statusResolved.Id;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id}/materials")]
    [Authorize(Roles = "Technician,Admin")]
    public async Task<ActionResult> AddMaterial(int id, InterventionMaterialCreateDto dto)
    {
        if (dto.Quantity <= 0)
            return BadRequest("Quantity must be greater than zero.");

        var intervention = await _context.Interventions
            .Include(i => i.WorkAssignment)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (intervention is null)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.EmployeeId != intervention.WorkAssignment.TechnicianId &&
            !User.IsInRole("Admin"))
            return Forbid();

        var material = new InterventionMaterial
        {
            InterventionId = id,
            MaterialId = dto.MaterialId,
            Quantity = dto.Quantity,
            MaterialUnitId = dto.MaterialUnitId
        };

        _context.InterventionMaterials.Add(material);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("list")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<List<InterventionListDto>>> GetList(
        [FromQuery] InterventionFilterDto filter)
    {
        var query = _context.Interventions
            .Include(i => i.WorkAssignment)
                .ThenInclude(a => a.FaultReport)
                    .ThenInclude(f => f.Location)
            .Include(i => i.WorkAssignment)
                .ThenInclude(a => a.Technician)
            .Include(i => i.InterventionStatus)
            .Include(i => i.Materials)
            .AsQueryable();

        // Filtriranje
        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            var search = filter.SearchText.ToLower();
            query = query.Where(i =>
                i.WorkAssignment.FaultReport.Title.ToLower().Contains(search) ||
                (i.Note != null && i.Note.ToLower().Contains(search)) ||
                i.WorkAssignment.FaultReportId.ToString().Contains(search));
        }

        if (filter.StatusId.HasValue)
            query = query.Where(i => i.InterventionStatusId == filter.StatusId);

        if (filter.TechnicianId.HasValue)
            query = query.Where(i =>
                i.WorkAssignment.TechnicianId == filter.TechnicianId);

        if (filter.DateFrom.HasValue)
            query = query.Where(i => i.CreatedAt >= filter.DateFrom);

        if (filter.DateTo.HasValue)
            query = query.Where(i => i.CreatedAt <= filter.DateTo);

        // Sortiranje
        query = filter.SortBy switch
        {
            "status" => filter.SortDescending
                ? query.OrderByDescending(i => i.InterventionStatusId)
                : query.OrderBy(i => i.InterventionStatusId),
            "technician" => filter.SortDescending
                ? query.OrderByDescending(i => i.WorkAssignment.Technician.LastName)
                : query.OrderBy(i => i.WorkAssignment.Technician.LastName),
            "duration" => filter.SortDescending
                ? query.OrderByDescending(i => i.DurationMinutes)
                : query.OrderBy(i => i.DurationMinutes),
            _ => filter.SortDescending
                ? query.OrderByDescending(i => i.CreatedAt)
                : query.OrderBy(i => i.CreatedAt)
        };

        var interventions = await query
            .Select(i => new InterventionListDto
            {
                Id = i.Id,
                WorkAssignmentId = i.WorkAssignmentId,
                FaultReportId = i.WorkAssignment.FaultReportId,
                FaultReportTitle = i.WorkAssignment.FaultReport.Title,
                LocationName = i.WorkAssignment.FaultReport.Location.Name,
                TechnicianId = i.WorkAssignment.TechnicianId,
                TechnicianName = i.WorkAssignment.Technician.FirstName
                    + " " + i.WorkAssignment.Technician.LastName,
                InterventionStatusId = i.InterventionStatusId,
                InterventionStatusName = i.InterventionStatus.Name,
                StartedAt = i.StartedAt,
                FinishedAt = i.FinishedAt,
                DurationMinutes = i.DurationMinutes,
                Note = i.Note,
                MaterialCount = i.Materials.Count,
                CreatedAt = i.CreatedAt
            })
            .ToListAsync();

        return Ok(interventions);
    }
}