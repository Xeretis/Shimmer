using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Shimmer.Core;
using Shimmer.Services;

namespace Shimmer;

public static class ShimmerServiceCollectionExtension
{
    /// <summary>
    ///     Adds Quartz and Shimmer to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="quartzConfigurator">The quartz configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddShimmer(this IServiceCollection services,
        Action<IServiceCollectionQuartzConfigurator>? quartzConfigurator = null)
    {
        services.AddQuartz(quartzConfigurator);

        services.AddSingleton<IShimmerJobManager, ShimmerJobManager>();
        services.AddTransient<IShimmerJobFactory, ShimmerJobFactory>();

        return services;
    }

    /// <summary>
    ///     Automatically discovers and adds all jobs in the assembly to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembly">The assembly to scan.</param>
    /// <param name="lifetime">The default lifetime of jobs.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection DiscoverJobs(this IServiceCollection services, Assembly assembly,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var jobs = new List<Type>();

        foreach (var type in assembly.GetTypes())
        {
            if (type.BaseType is { IsGenericType: true } &&
                type.BaseType.GetGenericTypeDefinition() == typeof(ShimmerJob<>))
            {
                var dataType = type.BaseType!.GetGenericArguments()[0];

                var method = typeof(ShimmerServiceCollectionExtension).GetMethods().First(m =>
                    m.Name.Equals(nameof(AddShimmerJob)) && m.IsGenericMethod && m.GetGenericArguments().Length == 2);
                var generic = method!.MakeGenericMethod(type, dataType);

                generic.Invoke(null, new object[] { services, lifetime });
            }

            if (type.BaseType == typeof(ShimmerJob))
            {
                var method = typeof(ShimmerServiceCollectionExtension).GetMethods().First(m =>
                    m.Name.Equals(nameof(AddShimmerJob)) && m.IsGenericMethod && m.GetGenericArguments().Length == 1);
                var generic = method!.MakeGenericMethod(type);

                generic.Invoke(null, new object[] { services, lifetime });
            }
        }

        return services;
    }

    /// <summary>
    ///     Adds a job to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The job lifetime.</param>
    /// <typeparam name="T">The type of the job.</typeparam>
    /// <typeparam name="TData">The type of the job data.</typeparam>
    public static void AddShimmerJob<T, TData>(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped) where T : ShimmerJob<TData> where TData : class
    {
        services.Add(new ServiceDescriptor(typeof(T), p =>
        {
            var job = (T)ActivatorUtilities.CreateInstance(p, typeof(T));

            job.shimmerJobManager = p.GetRequiredService<IShimmerJobManager>();

            return job;
        }, lifetime));
    }

    /// <summary>
    ///     Adds a job to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The job lifetime.</param>
    /// <typeparam name="T">The type of the job.</typeparam>
    public static void AddShimmerJob<T>(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped) where T : ShimmerJob
    {
        services.Add(new ServiceDescriptor(typeof(T), p =>
        {
            var job = (T)ActivatorUtilities.CreateInstance(p, typeof(T));

            job.shimmerJobManager = p.GetRequiredService<IShimmerJobManager>();

            return job;
        }, lifetime));
    }
}