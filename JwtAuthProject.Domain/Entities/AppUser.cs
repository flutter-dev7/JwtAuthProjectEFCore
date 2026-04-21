using Microsoft.AspNetCore.Identity;

namespace JwtAuthProject.Domain.Entities;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    
    // Navigation properties
    public ICollection<VerificationCode> VerificationCodes { get; set; } = [];
}
