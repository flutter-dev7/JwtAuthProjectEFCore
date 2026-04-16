using System;
using System.ComponentModel.DataAnnotations;

namespace JwtAuthProject.Application.DTOs.Auth.Request;

public class ResetPasswordDto
{
    [Required]
    [MaxLength(150)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;

    [MinLength(6)]
    [Compare("NewPassword")]
    public string ConfirmPassword { get; set; } = string.Empty;
}