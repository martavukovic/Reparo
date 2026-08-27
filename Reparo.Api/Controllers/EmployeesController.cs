using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reparo.Api.Data;
using Reparo.Shared.DTOs;
using Reparo.Shared.Models;

namespace Reparo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Manager")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _context;

    public EmployeesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<EmployeeDto>>> GetAll()
    {
        var employees = await _context.Employees
            .Include(e => e.Location)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                LocationId = e.LocationId,
                LocationName = e.Location.Name,
                IsTechnician = e.IsTechnician,
                IsActive = e.IsActive,
                IsAvailable = e.IsAvailable
            })
            .ToListAsync();

        return Ok(employees);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDto>> GetById(int id)
    {
        var employee = await _context.Employees
            .Include(e => e.Location)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee is null)
            return NotFound();

        return Ok(new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            LocationId = employee.LocationId,
            LocationName = employee.Location.Name,
            IsTechnician = employee.IsTechnician,
            IsActive = employee.IsActive,
            IsAvailable = employee.IsAvailable
        });
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create(EmployeeCreateDto dto)
    {
        var employee = new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            LocationId = dto.LocationId,
            IsTechnician = dto.IsTechnician,
            IsActive = dto.IsActive,
            IsAvailable = dto.IsAvailable
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById),
            new { id = employee.Id },
            new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                LocationId = employee.LocationId,
                IsTechnician = employee.IsTechnician,
                IsActive = employee.IsActive,
                IsAvailable = employee.IsAvailable
            });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, EmployeeCreateDto dto)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee is null)
            return NotFound();

        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.LocationId = dto.LocationId;
        employee.IsTechnician = dto.IsTechnician;
        employee.IsActive = dto.IsActive;
        employee.IsAvailable = dto.IsAvailable;

        await _context.SaveChangesAsync();
        return NoContent();
    }
}