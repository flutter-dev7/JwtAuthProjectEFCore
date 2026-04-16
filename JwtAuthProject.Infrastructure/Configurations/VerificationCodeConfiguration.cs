using System;
using JwtAuthProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JwtAuthProject.Infrastructure.Configurations;

public class VerificationCodeConfiguration : IEntityTypeConfiguration<VerificationCode>
{
    public void Configure(EntityTypeBuilder<VerificationCode> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Code)
        .IsRequired()
        .HasMaxLength(6);

        builder.Property(vc => vc.Expiration)
            .IsRequired();

        builder.HasIndex(vc => vc.Code);

        builder.HasOne(v => v.User)
        .WithMany(u => u.VerificationCodes)
        .HasForeignKey(v => v.UserId)
        .OnDelete(DeleteBehavior.Cascade);
    }
}
