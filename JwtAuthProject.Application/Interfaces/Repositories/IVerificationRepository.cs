using JwtAuthProject.Domain.Entities;

namespace JwtAuthProject.Application.Interfaces.Repositories;

public interface IVerificationCodeRepository
{
    Task AddAsync(VerificationCode code);
    Task<VerificationCode?> GetLatestVerificationCodeAsync(string userId);
    Task<VerificationCode?> GetVerificationCodeAsync(int id);
}