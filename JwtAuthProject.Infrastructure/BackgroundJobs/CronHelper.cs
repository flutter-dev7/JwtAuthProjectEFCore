using System;

namespace JwtAuthProject.Infrastructure.BackgroundJobs;

public static class CronHelper
{
    public static TimeSpan GetDelayToNextRun(int minutesInterval)
    {
        var now = DateTime.UtcNow;

        var next = now
            .AddMinutes(minutesInterval - (now.Minute % minutesInterval))
            .AddSeconds(-now.Second)
            .AddMilliseconds(-now.Millisecond);

        return next - now;
    }
}