using System.ComponentModel.DataAnnotations;

namespace JwtAuthProject.Application.DTOs.Auth.Request;

public class RegisterDto
{
    [Required]
    [MaxLength(150)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
