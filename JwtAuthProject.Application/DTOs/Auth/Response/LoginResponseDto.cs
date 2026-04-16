using JwtAuthProject.Application.DTOs.Users.Response;

namespace JwtAuthProject.Application.DTOs.Auth.Response;

public class LoginResponseDto : GetUserDto
{
    public string Token { get; set; } = string.Empty;
}
