using System;
using System.Security.Claims;
using JwtAuthProject.Application.DTOs.Auth.Request;
using JwtAuthProject.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthProject.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : BaseController
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginDto loginDto)
    {
        var result = await _service.LoginAsync(loginDto);

        return !result.IsSuccess ? HandleError(result) : Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterDto registerDto)
    {
        var register = await _service.RegisterAsync(registerDto);

        return !register.IsSuccess ? HandleError(register) : Created("", register);
    }

    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        var change = await _service.ChangePasswordAsync(userId, changePasswordDto);

        return !change.IsSuccess ? HandleError(change) : Ok(change);
    }

    [HttpPost("send-email")]
    public async Task<IActionResult> SendEmailAsync(SendEmailDto sendEmailDto)
    {
        var res = await _service.SendEmailAsync(sendEmailDto);

        return !res.IsSuccess ? HandleError(res) : Ok(res);
    }

    [HttpPost("verify-code")]
    public async Task<IActionResult> VerifyCodeAsync(VerifyCodeDto verifyCodeDto)
    {
        var res = await _service.VerifyCodeAsync(verifyCodeDto);

        return !res.IsSuccess ? HandleError(res) : Ok(res);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        var res = await _service.ResetPasswordAsync(resetPasswordDto);

        return !res.IsSuccess ? HandleError(res) : Ok(res);
    }
}
