using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Shimmer.AspNetCore;

public static class ShimmerAspNetCoreServiceCollectionExtension
{
    /// <summary>
    ///     Adds the Quartz hosted service to the service collection.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="quartzConfigurator">The quartz configuration for the hosted service.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddShimmerHostedService(this IServiceCollection services,
        Action<QuartzHostedServiceOptions>? quartzConfigurator = null)
    {
        services.AddQuartzHostedService(quartzConfigurator);

        return services;
    }
}