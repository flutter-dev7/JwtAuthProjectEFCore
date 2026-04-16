using JwtAuthProject.Domain.Entities;

namespace JwtAuthProject.Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(AppUser user, IList<string> roles);
}
