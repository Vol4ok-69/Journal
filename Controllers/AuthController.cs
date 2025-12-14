// Controllers/AuthController.cs
using JournalApi.DTOs.Auth;
using JournalApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JournalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
    {
        var result = await _authService.LoginAsync(request);
        if (result == null)
        {
            return Unauthorized(new { message = "Invalid login or password." });
        }

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO request)
    {
        var result = await _authService.RefreshTokenAsync(request);
        if (result == null)
        {
            return Unauthorized(new { message = "Invalid or expired refresh token." });
        }

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
    {
        var success = await _authService.RegisterAsync(request);
        if (!success)
        {
            return BadRequest(new { message = "Registration failed. Please check your data." });
        }

        return Ok(new { message = "User registered successfully." });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDTO request)
    {
        var success = await _authService.LogoutAsync(request.RefreshToken);
        if (!success)
        {
            return BadRequest(new { message = "Logout failed. Token may be invalid or already revoked." });
        }

        return Ok(new { message = "Logged out successfully." });
    }
}