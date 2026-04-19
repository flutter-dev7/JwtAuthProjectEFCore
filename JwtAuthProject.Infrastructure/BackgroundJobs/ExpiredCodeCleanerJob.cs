using JwtAuthProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthProject.Infrastructure.BackgroundJobs;

public class ExpiredCodeCleanerJob
{
    private readonly AppDbContext _context;

    public ExpiredCodeCleanerJob(AppDbContext context)
    {
        _context = context;
    }

    public async Task CleanAsync()
    {
        var deletedCount = await _context.VerificationCodes
            .Where(c => c.Expiration < DateTime.UtcNow)
            .ExecuteDeleteAsync();

        Console.WriteLine($"Deleted {deletedCount} expired codes");
    }
}