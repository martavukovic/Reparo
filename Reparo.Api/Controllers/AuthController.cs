using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Reparo.Api.Models;
using Reparo.Api.Services;
using Reparo.Shared.DTOs;

namespace Reparo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly JwtService _jwtService;

    public AuthController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        JwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoggedUserDto>> Login(LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Unauthorized("Pogrešan email ili lozinka.");

        var result = await _signInManager.CheckPasswordSignInAsync(
            user, request.Password, false);

        if (!result.Succeeded)
            return Unauthorized("Pogrešan email ili lozinka.");

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateToken(user, roles);

        return Ok(new LoggedUserDto
        {
            Id = user.Id,
            Email = user.Email!,
            DisplayName = user.UserName ?? user.Email!,
            Roles = roles.ToList(),
            EmployeeId = user.EmployeeId,
            Token = token
        });
    }
}