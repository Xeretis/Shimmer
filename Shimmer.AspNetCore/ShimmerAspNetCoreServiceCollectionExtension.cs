using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Shimmer.AspNetCore;

public static class ShimmerAspNetCoreServiceCollectionExtension
{
    public static IServiceCollection AddShimmerHostedService(this IServiceCollection services,
        Action<QuartzHostedServiceOptions>? quartzConfigurator = null)
    {
        services.AddQuartzHostedService(quartzConfigurator);

        return services;
    }
}