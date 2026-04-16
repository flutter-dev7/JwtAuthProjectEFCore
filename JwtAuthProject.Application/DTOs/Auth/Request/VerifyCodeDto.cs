using System;
using System.ComponentModel.DataAnnotations;

namespace JwtAuthProject.Application.DTOs.Auth.Request;

public class VerifyCodeDto
{
    [Required]
    [MaxLength(150)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Code { get; set; } = string.Empty;
}
