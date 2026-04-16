using System;
using JwtAuthProject.Application.Common;
using JwtAuthProject.Application.DTOs.Users.Request;
using JwtAuthProject.Application.DTOs.Users.Response;
using JwtAuthProject.Application.Interfaces.Services;
using JwtAuthProject.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JwtAuthProject.Domain.Enums;

namespace JwtAuthProject.Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Result<List<GetUserDto>>> GetAllUsersAsync()
    {
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

        return Result<List<GetUserDto>>.Ok(result);
    }

    public async Task<Result<GetUserDto?>> GetUserByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return Result<GetUserDto?>.Fail("User not found", ErrorType.NotFound);

        var roles = await _userManager.GetRolesAsync(user);

        return Result<GetUserDto?>.Ok(new GetUserDto
        {
            Id = user.Id,
            Username = user.UserName!,
            Email = user.Email!,
            Role = roles.FirstOrDefault() ?? "User"
        });
    }

    public async Task<Result<UpdateUserResponseDto>> UpdateUserAsync(string id, UpdateUserDto updateUserDto)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return Result<UpdateUserResponseDto>.Fail("User not found", ErrorType.NotFound);

        user.UserName = updateUserDto.Username;
        user.Email = updateUserDto.Email;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            return Result<UpdateUserResponseDto>.Fail(errors, ErrorType.Validation);
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, updateUserDto.Role);

        return Result<UpdateUserResponseDto>.Ok(new UpdateUserResponseDto
        {
            Id = user.Id,
            Username = user.UserName!,
            Email = user.Email!,
            Role = updateUserDto.Role
        });
    }

    public async Task<Result<DeleteUserResponseDto>> DeleteUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return Result<DeleteUserResponseDto>.Fail("User not found", ErrorType.NotFound);

        var roles = await _userManager.GetRolesAsync(user);

        await _userManager.DeleteAsync(user);

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

        var RoleExists = await _roleManager.RoleExistsAsync(role);

        if (!RoleExists)
            return Result<bool>.Fail("Role does not exists", ErrorType.Validation);

        var result = await _userManager.AddToRoleAsync(user, role);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<bool>.Fail(errors, ErrorType.Validation);
        }

        return Result<bool>.Ok(true);
    }
}
