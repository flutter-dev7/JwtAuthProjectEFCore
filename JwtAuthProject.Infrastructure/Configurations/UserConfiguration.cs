using JwtAuthProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JwtAuthProject.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(u => u.UserName)
        .IsRequired()
        .HasMaxLength(150);

        builder.Property(u => u.Email)
        .IsRequired()
        .HasMaxLength(150);

        builder.HasIndex(u => u.Email).IsUnique();
    }
}
