using JwtAuthProject.Application.Common;
using JwtAuthProject.Application.DTOs.Users.Request;
using JwtAuthProject.Application.DTOs.Users.Response;
using JwtAuthProject.Application.Interfaces.Services;
using JwtAuthProject.Domain.Entities;
using JwtAuthProject.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthProject.Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ICacheService _cache;

    public UserService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ICacheService cache)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _cache = cache;
    }

    public async Task<Result<List<GetUserDto>>> GetAllUsersAsync()
    {
        var cached = await _cache.GetAsync<List<GetUserDto>>("all_users");
        if (cached != null)
            return Result<List<GetUserDto>>.Ok(cached);

        var users = await _userManager.Users.ToListAsync();

        var result = new List<GetUserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new GetUserDto
            {
                Id = user.Id,
                Username = user.UserName!,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "User"
            });
        }

        await _cache.SetAsync("all_users", result, TimeSpan.FromMinutes(10)); 

        return Result<List<GetUserDto>>.Ok(result);
    }

    public async Task<Result<GetUserDto?>> GetUserByIdAsync(string id)
    {
        var cached = await _cache.GetAsync<GetUserDto>($"user_{id}");
        if (cached != null)
            return Result<GetUserDto?>.Ok(cached);

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return Result<GetUserDto?>.Fail("User not found", ErrorType.NotFound);

        var roles = await _userManager.GetRolesAsync(user);

        var dto = new GetUserDto
        {
            Id = user.Id,
            Username = user.UserName!,
            Email = user.Email!,
            Role = roles.FirstOrDefault() ?? "User"
        };

        await _cache.SetAsync($"user_{id}", dto, TimeSpan.FromMinutes(10));

        return Result<GetUserDto?>.Ok(dto);
    }

    public async Task<Result<UpdateUserResponseDto>> UpdateUserAsync(string id, UpdateUserDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            return Result<UpdateUserResponseDto>.Fail("Username cannot be empty", ErrorType.Validation);

        if (string.IsNullOrWhiteSpace(request.Email))
            return Result<UpdateUserResponseDto>.Fail("Email cannot be empty", ErrorType.Validation);

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return Result<UpdateUserResponseDto>.Fail("User not found", ErrorType.NotFound);

        user.UserName = request.Username;
        user.Email = request.Email;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            return Result<UpdateUserResponseDto>.Fail(errors, ErrorType.Validation);
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, request.Role);

        await _cache.RemoveAsync("all_users");
        await _cache.RemoveAsync($"user_{id}");

        return Result<UpdateUserResponseDto>.Ok(new UpdateUserResponseDto
        {
            Id = user.Id,
            Username = user.UserName!,
            Email = user.Email!,
            Role = request.Role
        });
    }

    public async Task<Result<DeleteUserResponseDto>> DeleteUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return Result<DeleteUserResponseDto>.Fail("User not found", ErrorType.NotFound);

        var roles = await _userManager.GetRolesAsync(user);

        var deleteResult = await _userManager.DeleteAsync(user);
        if (!deleteResult.Succeeded)
        {
            var errors = string.Join(", ", deleteResult.Errors.Select(e => e.Description));
            return Result<DeleteUserResponseDto>.Fail(errors, ErrorType.Unknown);
        }

        await _cache.RemoveAsync("all_users");
        await _cache.RemoveAsync($"user_{id}");

        return Result<DeleteUserResponseDto>.Ok(new DeleteUserResponseDto
        {
            Id = user.Id,
            Username = user.UserName!,
            Email = user.Email!,
            Role = roles.FirstOrDefault() ?? "User"
        });
    }

    public async Task<Result<bool>> AssisgnRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<bool>.Fail("User not found", ErrorType.NotFound);

        var roleExists = await _roleManager.RoleExistsAsync(role);
        if (!roleExists)
            return Result<bool>.Fail("Role does not exist", ErrorType.Validation);

        var alreadyInRole = await _userManager.IsInRoleAsync(user, role);
        if (alreadyInRole)
            return Result<bool>.Fail($"User already has role '{role}'", ErrorType.Validation);

        var result = await _userManager.AddToRoleAsync(user, role);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<bool>.Fail(errors, ErrorType.Validation);
        }

        await _cache.RemoveAsync("all_users");
        await _cache.RemoveAsync($"user_{userId}");

        return Result<bool>.Ok(true);
    }
}