using Quartz;
using Shimmer.Core;

namespace Shimmer.WebApiExample.Jobs;

public class CheckWeatherJob(ILogger<CheckWeatherJob> logger) : ShimmerJob
{
    protected override async Task Process(IJobExecutionContext context)
    {
        logger.LogInformation("Checking weather...");
    }
}