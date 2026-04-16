using JwtAuthProject.Domain.Entities;
using JwtAuthProject.Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthProject.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public DbSet<VerificationCode> VerificationCodes { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); 

        builder.ApplyConfiguration(new UserConfiguration());
        builder.ApplyConfiguration(new VerificationCodeConfiguration());
    }
}
