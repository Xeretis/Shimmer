using Quartz;
using Shimmer.Core;

namespace Shimmer.WebApiExample.Jobs;

public class SendWeatherNotificationJob(ILogger<SendWeatherNotificationJob> logger) : ShimmerJob
{
    protected override async Task Process(IJobExecutionContext context)
    {
        logger.LogInformation("Sending weather notification...");
    }
}