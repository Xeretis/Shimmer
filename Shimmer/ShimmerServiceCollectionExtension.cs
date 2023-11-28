using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Shimmer.Core;
using Shimmer.Services;

namespace Shimmer;

public static class ShimmerServiceCollectionExtension
{
    public static IServiceCollection AddShimmer(this IServiceCollection services,
        Action<IServiceCollectionQuartzConfigurator>? quartzConfigurator = null)
    {
        services.AddQuartz(quartzConfigurator);

        services.AddSingleton<IShimmerJobManager, ShimmerJobManager>();
        services.AddTransient<IShimmerJobFactory, ShimmerJobFactory>();

        return services;
    }

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