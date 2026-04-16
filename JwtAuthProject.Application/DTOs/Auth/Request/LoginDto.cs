using System.ComponentModel.DataAnnotations;

namespace JwtAuthProject.Application.DTOs.Auth.Request;

public class LoginDto
{
    [Required]
    [MaxLength(150)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}
