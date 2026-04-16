using JwtAuthProject.Application.Common;
using JwtAuthProject.Application.DTOs.Users.Request;
using JwtAuthProject.Application.DTOs.Users.Response;

namespace JwtAuthProject.Application.Interfaces.Services;

public interface IUserService
{
    Task<Result<List<GetUserDto>>> GetAllUsersAsync();
    Task<Result<GetUserDto?>> GetUserByIdAsync(string id);
    Task<Result<UpdateUserResponseDto>> UpdateUserAsync(string id, UpdateUserDto updateUserDto);
    Task<Result<DeleteUserResponseDto>> DeleteUserAsync(string id);
    Task<Result<bool>> AssisgnRoleAsync(string userId, string role);
}
