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

    private void SetRefreshTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(14)
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
    {
        var result = await _authService.LoginAsync(request);

        if (string.IsNullOrEmpty(result.AccessToken))
        {
            return Unauthorized(new { message = result.Message ?? "Invalid login or password." });
        }

        SetRefreshTokenCookie(result.RefreshToken!);
        return Ok(new { accessToken = result.AccessToken });
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO request)
    {
        var result = await _authService.RefreshTokenAsync(request);

        if (result == null || string.IsNullOrEmpty(result.AccessToken))
        {
            return Unauthorized(new { message = result?.Message ?? "Invalid or expired refresh token." });
        }

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
    {
        var result = await _authService.RegisterAsync(request);

        if (!result.Result)
        {
            return BadRequest(new { message = result.Message });
        }

        return Ok(new { message = result.Message });
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