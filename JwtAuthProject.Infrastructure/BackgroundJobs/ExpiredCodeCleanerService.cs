using System;
using JwtAuthProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JwtAuthProject.Infrastructure.BackgroundJobs;

public class ExpiredCodeCleanerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    public ExpiredCodeCleanerService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var expiredCodes = await context.VerificationCodes
                .Where(c => c.Expiration < DateTime.UtcNow)
                .ToListAsync();

            if (expiredCodes.Any())
            {
                context.VerificationCodes.RemoveRange(expiredCodes);
                await context.SaveChangesAsync();
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
