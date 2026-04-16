using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JwtAuthProject.Domain.Entities;

public class VerificationCode
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }

    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public AppUser User { get; set; } = null!;
}
