using System;
using JwtAuthProject.Application.DTOs.Users.Request;
using JwtAuthProject.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthProject.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController : BaseController
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        var users = await _service.GetAllUsersAsync();

        return !users.IsSuccess ? HandleError(users) : Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserByIdAsync(string id)
    {
        var user = await _service.GetUserByIdAsync(id);

        return !user.IsSuccess ? HandleError(user) : Ok(user);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUserAsync(string id, UpdateUserDto updateUserDto)
    {
        var res = await _service.UpdateUserAsync(id, updateUserDto);

        return !res.IsSuccess ? HandleError(res) : Ok(res);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUserAsync(string id)
    {
        var res = await _service.DeleteUserAsync(id);

        return !res.IsSuccess ? HandleError(res) : Ok(res);
    }

    [HttpPost("{id}/assign-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRoleAsync(string id, string role)
    {
        var res = await _service.AssisgnRoleAsync(id, role);

        return !res.IsSuccess ? HandleError(res) : Ok(res);
    }
}
