using System.Net.Mail;
using JwtAuthProject.Application.Common;
using JwtAuthProject.Application.DTOs.Auth.Request;
using JwtAuthProject.Application.DTOs.Auth.Response;
using JwtAuthProject.Application.Interfaces.Repositories;
using JwtAuthProject.Application.Interfaces.Services;
using JwtAuthProject.Domain.Constrains;
using JwtAuthProject.Domain.Entities;
using JwtAuthProject.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace JwtAuthProject.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly IVerificationCodeRepository _verificationCodeRepository;

    public AuthService(UserManager<AppUser> userManager, IJwtService jwtService, IEmailService emailService, IVerificationCodeRepository verificationCodeRepository)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _emailService = emailService;
        _verificationCodeRepository = verificationCodeRepository;
    }

    public async Task<Result<LoginResponseDto>> LoginAsync(LoginDto loginDto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
                return Result<LoginResponseDto>.Fail($"User with Email: {loginDto.Email} not found", ErrorType.NotFound);

            var checkPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!checkPassword)
                return Result<LoginResponseDto>.Fail("Invalid password or email", ErrorType.Validation);

            var roles = await _userManager.GetRolesAsync(user);

            var token = _jwtService.GenerateToken(user, roles);

            return Result<LoginResponseDto>.Ok(new LoginResponseDto
            {
                Id = user.Id,
                Username = user.UserName!,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "User",
                Token = token
            });
        }
        catch (System.Exception ex)
        {
            return Result<LoginResponseDto>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }

    public async Task<Result<RegisterReponseDto>> RegisterAsync(RegisterDto registerDto)
    {
        if (registerDto.Password.Length < 6)
            return Result<RegisterReponseDto>.Fail("Password cannot be less than 6", ErrorType.Validation);

        if (registerDto.Password != registerDto.ConfirmPassword)
            return Result<RegisterReponseDto>.Fail("Password do not match", ErrorType.Validation);
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);

            if (existingUser != null)
                return Result<RegisterReponseDto>.Fail("User already exists", ErrorType.Conflict);

            var user = new AppUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));

                return Result<RegisterReponseDto>.Fail(errors, ErrorType.Validation);
            }

            await _userManager.AddToRoleAsync(user, UserRole.User);

            return Result<RegisterReponseDto>.Ok(new RegisterReponseDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Role = "User"
            });
        }
        catch (System.Exception ex)
        {
            return Result<RegisterReponseDto>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }

    public async Task<Result<bool>> ChangePasswordAsync(string id, ChangePasswordDto changePasswordDto)
    {
        if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
            return Result<bool>.Fail("Password do not match", ErrorType.Validation);
        try
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return Result<bool>.Fail("User not found", ErrorType.NotFound);

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result<bool>.Fail(errors, ErrorType.Validation);
            }

            return Result<bool>.Ok(true);
        }
        catch (System.Exception ex)
        {
            return Result<bool>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }

    public async Task<Result<bool>> SendEmailAsync(SendEmailDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Result<bool>.Fail("User not found", ErrorType.NotFound);

        var code = new Random().Next(100_000, 999_999).ToString();
        var htmlBody = $@"
    <!DOCTYPE html>
    <html>
    <head>
        <meta charset='UTF-8'>
        <style>
            body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }}
            .container {{ max-width: 500px; margin: 40px auto; background: #ffffff; border-radius: 10px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.1); }}
            .header {{ background: linear-gradient(135deg, #667eea, #764ba2); padding: 30px; text-align: center; }}
            .header h1 {{ color: white; margin: 0; font-size: 24px; }}
            .body {{ padding: 30px; text-align: center; }}
            .body p {{ color: #555; font-size: 15px; line-height: 1.6; }}
            .code-box {{ display: inline-block; background: #f0f0f0; border: 2px dashed #667eea; border-radius: 8px; padding: 15px 40px; margin: 20px 0; }}
            .code {{ font-size: 36px; font-weight: bold; color: #667eea; letter-spacing: 8px; }}
            .warning {{ color: #999; font-size: 13px; margin-top: 10px; }}
            .footer {{ background: #f9f9f9; padding: 15px; text-align: center; color: #aaa; font-size: 12px; border-top: 1px solid #eee; }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'>
                <h1>🔐 Password Reset</h1>
            </div>
            <div class='body'>
                <p>Hello, <strong>{user.UserName}</strong>!</p>
                <p>We received a request to reset your password.<br>Use the code below:</p>
                <div class='code-box'>
                    <div class='code'>{code}</div>
                </div>
                <p class='warning'>⏱ This code expires in <strong>3 minutes</strong>.</p>
                <p class='warning'>If you did not request this, ignore this email.</p>
            </div>
            <div class='footer'>
                © 2026 JwtAuthApp — Do not reply to this email
            </div>
        </div>
    </body>
    </html>";
        try
        {
            var result = await _emailService.SendEmailAsync(request.Email, "Password reset", htmlBody);
            if (!result)
            {
                return Result<bool>.Fail("Failed to send email");
            }
        }
        catch (SmtpException e)
        {
            return Result<bool>.Fail("Failed to send email: " + e.Message);
        }

        var verificationCode = new VerificationCode
        {
            UserId = user.Id,
            Code = code,
            Expiration = DateTime.UtcNow.AddMinutes(3)
        };

        await _verificationCodeRepository.AddAsync(verificationCode);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<bool>> VerifyCodeAsync(VerifyCodeDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Result<bool>.Fail("User not found", ErrorType.NotFound);

        var lastCode = await _verificationCodeRepository
            .GetLatestVerificationCodeAsync(user.Id);

        if (lastCode == null)
            return Result<bool>.Fail("Code not found", ErrorType.NotFound);

        if (lastCode.Code != request.Code)
            return Result<bool>.Fail("Invalid code", ErrorType.Validation);

        if (lastCode.Expiration < DateTime.UtcNow)
            return Result<bool>.Fail("Code expired", ErrorType.Validation);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Result<bool>.Fail("User not found", ErrorType.NotFound);

        if (request.NewPassword != request.ConfirmPassword)
            return Result<bool>.Fail("Passwords do not match", ErrorType.Validation);

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(
            user,
            token,
            request.NewPassword
        );

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(x => x.Description));
            return Result<bool>.Fail(errors, ErrorType.Validation);
        }

        return Result<bool>.Ok(true);
    }
}
