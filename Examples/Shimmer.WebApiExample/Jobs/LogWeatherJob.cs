using Quartz;
using Shimmer.Core;
using Shimmer.WebApiExample.Jobs.Data;

namespace Shimmer.WebApiExample.Jobs;

public class LogWeatherJob(ILogger<LogWeatherJob> logger) : ShimmerJob<LogWeatherJobData>
{
    protected override async Task Process(LogWeatherJobData data, IJobExecutionContext context)
    {
        logger.LogInformation("Logging weather...");
        logger.LogInformation("{}", data.Forecasts.Select(f => f.Summary + " " + f.TemperatureC)
            .Aggregate((a, b) => a + ", " + b));
    }
}