using FinancePlatform.Application.DTOs.Auth;
using FinancePlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancePlatform.API.Controllers;

public class AuthController : ApiControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var result = await _auth.RegisterAsync(request);
        return CreatedAtAction(nameof(Me), result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await _auth.LoginAsync(request);
        return Ok(result);
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserProfileResponse>> Me()
    {
        var result = await _auth.GetProfileAsync(CurrentUserId);
        return Ok(result);
    }
}
