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
public class MaterialsController : ControllerBase
{
    private readonly AppDbContext _context;

    public MaterialsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<LookupDto>>> GetAll()
    {
        var materials = await _context.Materials
            .Where(m => m.IsActive)
            .Select(m => new LookupDto { Id = m.Id, Name = m.Name })
            .ToListAsync();

        return Ok(materials);
    }

    [HttpPost]
    public async Task<ActionResult<LookupDto>> Create([FromBody] CreateMaterialDto dto)
    {
        var existing = await _context.Materials
            .FirstOrDefaultAsync(m => m.Name.ToLower() == dto.Name.ToLower());

        if (existing is not null)
            return Ok(new LookupDto { Id = existing.Id, Name = existing.Name });

        var material = new Material
        {
            Name = dto.Name,
            IsActive = true
        };

        _context.Materials.Add(material);
        await _context.SaveChangesAsync();

        return Ok(new LookupDto { Id = material.Id, Name = material.Name });
    }
}