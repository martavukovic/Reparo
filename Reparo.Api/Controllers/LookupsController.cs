using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reparo.Api.Data;
using Reparo.Shared.DTOs;

namespace Reparo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LookupsController : ControllerBase
{
    private readonly AppDbContext _context;

    public LookupsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("location-types")]
    public async Task<ActionResult<List<LookupDto>>> GetLocationTypes()
    {
        var items = await _context.LocationTypes
            .Select(x => new LookupDto { Id = x.Id, Name = x.Name })
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("fault-types")]
    public async Task<ActionResult<List<LookupDto>>> GetFaultTypes()
    {
        var items = await _context.FaultTypes
            .Select(x => new LookupDto { Id = x.Id, Name = x.Name })
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("fault-priorities")]
    public async Task<ActionResult<List<LookupDto>>> GetFaultPriorities()
    {
        var items = await _context.FaultPriorities
            .Select(x => new LookupDto { Id = x.Id, Name = x.Name })
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("fault-statuses")]
    public async Task<ActionResult<List<LookupDto>>> GetFaultStatuses()
    {
        var items = await _context.FaultStatuses
            .Select(x => new LookupDto { Id = x.Id, Name = x.Name })
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("intervention-statuses")]
    public async Task<ActionResult<List<LookupDto>>> GetInterventionStatuses()
    {
        var items = await _context.InterventionStatuses
            .Select(x => new LookupDto { Id = x.Id, Name = x.Name })
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("material-units")]
    public async Task<ActionResult<List<LookupDto>>> GetMaterialUnits()
    {
        var items = await _context.MaterialUnits
            .Select(x => new LookupDto { Id = x.Id, Name = x.Name })
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("employees")]
    public async Task<ActionResult<List<LookupDto>>> GetEmployees()
    {
        var items = await _context.Employees
            .Where(e => e.IsActive)
            .Select(e => new LookupDto
            {
                Id = e.Id,
                Name = e.FirstName + " " + e.LastName
            })
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("technicians")]
    public async Task<ActionResult<List<LookupDto>>> GetTechnicians()
    {
        var items = await _context.Employees
            .Where(e => e.IsActive && e.IsTechnician && e.IsAvailable)
            .Select(e => new LookupDto
            {
                Id = e.Id,
                Name = e.FirstName + " " + e.LastName
            })
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("materials")]
    public async Task<ActionResult<List<LookupDto>>> GetMaterials()
    {
        var items = await _context.Materials
            .Where(m => m.IsActive)
            .Select(m => new LookupDto { Id = m.Id, Name = m.Name })
            .ToListAsync();
        return Ok(items);
    }

    [HttpPut("{type}/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Update(string type, int id, [FromBody] LookupDto dto)
    {
        switch (type)
        {
            case "fault-types":
                var ft = await _context.FaultTypes.FindAsync(id);
                if (ft is null) return NotFound();
                ft.Name = dto.Name;
                break;
            case "fault-priorities":
                var fp = await _context.FaultPriorities.FindAsync(id);
                if (fp is null) return NotFound();
                fp.Name = dto.Name;
                break;
            case "fault-statuses":
                var fs = await _context.FaultStatuses.FindAsync(id);
                if (fs is null) return NotFound();
                fs.Name = dto.Name;
                break;
            case "intervention-statuses":
                var is_ = await _context.InterventionStatuses.FindAsync(id);
                if (is_ is null) return NotFound();
                is_.Name = dto.Name;
                break;
            case "material-units":
                var mu = await _context.MaterialUnits.FindAsync(id);
                if (mu is null) return NotFound();
                mu.Name = dto.Name;
                break;
            case "location-types":
                var lt = await _context.LocationTypes.FindAsync(id);
                if (lt is null) return NotFound();
                lt.Name = dto.Name;
                break;
            default:
                return BadRequest("Unknown lookup type.");
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("technicians-by-location/{locationId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<List<LookupDto>>> GetTechniciansByLocation(int locationId)
    {
        var items = await _context.Employees
            .Where(e => e.IsActive && e.IsTechnician &&
                        e.IsAvailable && e.LocationId == locationId)
            .Select(e => new LookupDto
            {
                Id = e.Id,
                Name = e.FirstName + " " + e.LastName
            })
            .ToListAsync();

        return Ok(items);
    }
}