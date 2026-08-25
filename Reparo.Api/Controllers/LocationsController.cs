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
public class LocationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public LocationsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<LocationDto>>> GetAll()
    {
        var locations = await _context.Locations
            .Include(l => l.LocationType)
            .Where(l => !l.IsActive == false || l.IsActive)
            .Select(l => new LocationDto
            {
                Id = l.Id,
                Name = l.Name,
                Address = l.Address,
                LocationTypeId = l.LocationTypeId,
                LocationTypeName = l.LocationType.Name,
                IsActive = l.IsActive
            })
            .ToListAsync();

        return Ok(locations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LocationDto>> GetById(int id)
    {
        var location = await _context.Locations
            .Include(l => l.LocationType)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (location is null)
            return NotFound();

        return Ok(new LocationDto
        {
            Id = location.Id,
            Name = location.Name,
            Address = location.Address,
            LocationTypeId = location.LocationTypeId,
            LocationTypeName = location.LocationType.Name,
            IsActive = location.IsActive
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<LocationDto>> Create(LocationCreateDto dto)
    {
        var location = new Location
        {
            Name = dto.Name,
            Address = dto.Address,
            LocationTypeId = dto.LocationTypeId,
            IsActive = dto.IsActive
        };

        _context.Locations.Add(location);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById),
            new { id = location.Id },
            new LocationDto
            {
                Id = location.Id,
                Name = location.Name,
                Address = location.Address,
                LocationTypeId = location.LocationTypeId,
                IsActive = location.IsActive
            });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult> Update(int id, LocationCreateDto dto)
    {
        var location = await _context.Locations.FindAsync(id);

        if (location is null)
            return NotFound();

        location.Name = dto.Name;
        location.Address = dto.Address;
        location.LocationTypeId = dto.LocationTypeId;
        location.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("lookup")]
    public async Task<ActionResult<List<LookupDto>>> GetLookup()
    {
        var locations = await _context.Locations
            .Where(l => l.IsActive)
            .Select(l => new LookupDto { Id = l.Id, Name = l.Name })
            .ToListAsync();

        return Ok(locations);
    }
}