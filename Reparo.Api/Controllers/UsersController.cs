using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reparo.Api.Data;
using Reparo.Api.Models;
using Reparo.Shared.DTOs;

namespace Reparo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _context;

    public UsersController(UserManager<AppUser> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        var users = await _userManager.Users.ToListAsync();
        var result = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                EmployeeId = user.EmployeeId,
                Roles = roles.ToList()
            });
        }

        return Ok(result);
    }

    [HttpPut("{id}/employee")]
    public async Task<ActionResult> LinkEmployee(string id, [FromBody] int employeeId)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound();

        user.EmployeeId = employeeId;
        await _userManager.UpdateAsync(user);

        return NoContent();
    }

    [HttpPut("{id}/roles")]
    public async Task<ActionResult> UpdateRoles(string id, [FromBody] List<string> roles)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound();

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRolesAsync(user, roles);

        return NoContent();
    }

    [HttpPut("{id}/deactivate")]
    public async Task<ActionResult> Deactivate(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound();

        user.LockoutEnabled = true;
        user.LockoutEnd = DateTimeOffset.MaxValue;
        await _userManager.UpdateAsync(user);

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateUserDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser is not null)
            return BadRequest("User with this email already exists.");

        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            EmployeeId = dto.EmployeeId
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors.First().Description);

        if (dto.Roles.Any())
            await _userManager.AddToRolesAsync(user, dto.Roles);

        return Ok();
    }
}