using JwtAuthProject.Application.Common;
using JwtAuthProject.Application.DTOs.Auth.Request;
using JwtAuthProject.Application.DTOs.Auth.Response;

namespace JwtAuthProject.Application.Interfaces.Services;

public interface IAuthService
{
    Task<Result<LoginResponseDto>> LoginAsync(LoginDto loginDto);
    Task<Result<RegisterReponseDto>> RegisterAsync(RegisterDto registerDto);
    Task<Result<bool>> ChangePasswordAsync(string id, ChangePasswordDto changePasswordDto);
    Task<Result<bool>> SendEmailAsync(SendEmailDto request);
    Task<Result<bool>> VerifyCodeAsync(VerifyCodeDto request);
    Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto request);
}
