using System;
using JwtAuthProject.Domain.Constrains;
using JwtAuthProject.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace JwtAuthProject.Infrastructure.Seeds;

public static class UserSeeder
{
    public static async Task SeedAdminAsync(UserManager<AppUser> userManager)
    {
        var email = "admin@gmail.com";
        var password = "Admin@123";

        var admin = await userManager.FindByEmailAsync(email);

        if (admin == null)
        {
            var user = new AppUser
            {
                UserName = "admin",
                Email = email
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, UserRole.Admin);
            }
        }
    }
}
