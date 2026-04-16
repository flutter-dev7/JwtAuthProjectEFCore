using System;
using JwtAuthProject.Domain.Constrains;
using Microsoft.AspNetCore.Identity;

namespace JwtAuthProject.Infrastructure.Seeds;

public static class RoleSeeder
{
    public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles =
        {
            UserRole.Admin,
            UserRole.Manager,
            UserRole.User
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}