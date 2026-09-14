using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Reparo.Api.Data;
using Reparo.Api.Models;
using Reparo.Api.Services;
using Reparo.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Reparo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly JwtService _jwtService;
    private readonly AppDbContext _context;

    public AuthController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        JwtService jwtService,
        AppDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _context = context;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoggedUserDto>> Login(LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Unauthorized("Wrong email or password.");

        var result = await _signInManager.CheckPasswordSignInAsync(
            user, request.Password, false);

        if (!result.Succeeded)
            return Unauthorized("Wrong email or password.");

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateToken(user, roles);

        return Ok(new LoggedUserDto
        {
            Id = user.Id,
            Email = user.Email!,
            DisplayName = user.EmployeeId.HasValue
                ? await _context.Employees
                    .Where(e => e.Id == user.EmployeeId)
                    .Select(e => e.FirstName + " " + e.LastName)
                    .FirstOrDefaultAsync() ?? user.Email!
                : user.Email!,
            Roles = roles.ToList(),
            EmployeeId = user.EmployeeId,
            Token = token
        });
    }
}