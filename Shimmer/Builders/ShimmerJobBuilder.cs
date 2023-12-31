using System.Text.Json;
using Quartz;
using Shimmer.Core;

namespace Shimmer.Builders;

/// <summary>
///     The builder used to configure and build independent jobs.
/// </summary>
/// <param name="scheduler">The Quartz scheduler to use.</param>
/// <typeparam name="T">The type of the job.</typeparam>
/// <typeparam name="TData">The type of the job data.</typeparam>
public class ShimmerJobBuilder<T, TData>(IScheduler scheduler) : IShimmerJobBuilder<T, TData>
    where T : ShimmerJob<TData> where TData : class
{
    public bool dataProvided;

    public bool startScheduled;

    public JobBuilder JobBuilder { get; set; } = JobBuilder.Create<T>();
    public TriggerBuilder TriggerBuilder { get; set; } = TriggerBuilder.Create();

    public JobKey JobKey { get; set; } = new(Guid.NewGuid().ToString());

    /// <inheritdoc />
    public IShimmerJobBuilder<T, TData> Data(TData data)
    {
        JobBuilder.UsingJobData("data", JsonSerializer.Serialize(data));
        dataProvided = true;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobBuilder<T, TData> Group(string group)
    {
        JobKey.Group = group;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobBuilder<T, TData> Name(string name)
    {
        JobKey.Name = name;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobBuilder<T, TData> Description(string description)
    {
        JobBuilder.WithDescription(description);

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobBuilder<T, TData> Concurrent(bool concurrent = true)
    {
        JobBuilder.DisallowConcurrentExecution(!concurrent);

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobBuilder<T, TData> RequestRecovery(bool request = true)
    {
        JobBuilder.RequestRecovery(request);

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobBuilder<T, TData> Durable(bool store = true)
    {
        JobBuilder.StoreDurably(store);

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobBuilder<T, TData> Priority(int priority)
    {
        TriggerBuilder.WithPriority(priority);

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobBuilder<T, TData> Schedule(IScheduleBuilder scheduleBuilder)
    {
        TriggerBuilder.WithSchedule(scheduleBuilder);
        startScheduled = true;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobBuilder<T, TData> SimpleSchedule(Action<SimpleScheduleBuilder> configure)
    {
        TriggerBuilder.WithSimpleSchedule(configure);
        startScheduled = true;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobBuilder<T, TData> CronSchedule(string cronExpression)
    {
        TriggerBuilder.WithCronSchedule(cronExpression);
        startScheduled = true;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobBuilder<T, TData> StartAt(DateTimeOffset startTimeOffset)
    {
        TriggerBuilder.StartAt(startTimeOffset);
        startScheduled = true;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobBuilder<T, TData> EndAt(DateTimeOffset? endTimeOffset)
    {
        TriggerBuilder.EndAt(endTimeOffset);

        return this;
    }

    /// <inheritdoc />
    public Task<DateTimeOffset> FireAsync(CancellationToken cancellationToken = default)
    {
        if (!CheckDataProvided())
            throw new InvalidOperationException("No data has been provided. Did you forget to call Data()?");

        JobBuilder.WithIdentity(JobKey);

        if (!CheckStartScheduled())
            TriggerBuilder.StartNow();

        var job = JobBuilder.Build();
        var trigger = TriggerBuilder.Build();

        return scheduler.ScheduleJob(job, trigger, cancellationToken);
    }

    public bool CheckDataProvided()
    {
        return dataProvided;
    }

    public bool CheckStartScheduled()
    {
        return startScheduled;
    }
}

/// <summary>
///     The builder used to configure and build independent jobs.
/// </summary>
/// <param name="scheduler">The Quartz scheduler to use.</param>
/// <typeparam name="T">The type of the job.</typeparam>
public class ShimmerJobBuilder<T>(IScheduler scheduler) : IShimmerJobBuilder<T> where T : ShimmerJob
{
    public bool startScheduled;

    public JobBuilder JobBuilder { get; set; } = JobBuilder.Create<T>();
    public TriggerBuilder TriggerBuilder { get; set; } = TriggerBuilder.Create();

    public JobKey JobKey { get; set; } = new(Guid.NewGuid().ToString());

    /// <inheritdoc />
    public IShimmerJobBuilder<T> Group(string group)
    {
        JobKey.Group = group;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobBuilder<T> Name(string name)
    {
        JobKey.Name = name;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobBuilder<T> Description(string description)
    {
        JobBuilder.WithDescription(description);

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobBuilder<T> Concurrent(bool concurrent = true)
    {
        JobBuilder.DisallowConcurrentExecution(!concurrent);

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobBuilder<T> RequestRecovery(bool request = true)
    {
        JobBuilder.RequestRecovery(request);

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobBuilder<T> Durable(bool store = true)
    {
        JobBuilder.StoreDurably(store);

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobBuilder<T> Priority(int priority)
    {
        TriggerBuilder.WithPriority(priority);

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobBuilder<T> Schedule(IScheduleBuilder scheduleBuilder)
    {
        TriggerBuilder.WithSchedule(scheduleBuilder);
        startScheduled = true;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobBuilder<T> SimpleSchedule(Action<SimpleScheduleBuilder> configure)
    {
        TriggerBuilder.WithSimpleSchedule(configure);
        startScheduled = true;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobBuilder<T> CronSchedule(string cronExpression)
    {
        TriggerBuilder.WithCronSchedule(cronExpression);
        startScheduled = true;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobBuilder<T> StartAt(DateTimeOffset startTimeOffset)
    {
        TriggerBuilder.StartAt(startTimeOffset);
        startScheduled = true;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobBuilder<T> EndAt(DateTimeOffset? endTimeOffset)
    {
        TriggerBuilder.EndAt(endTimeOffset);

        return this;
    }

    /// <inheritdoc />
    public virtual Task<DateTimeOffset> FireAsync(CancellationToken cancellationToken = default)
    {
        JobBuilder.WithIdentity(JobKey);

        if (!CheckStartScheduled())
            TriggerBuilder.StartNow();

        var job = JobBuilder.Build();
        var trigger = TriggerBuilder.Build();

        return scheduler.ScheduleJob(job, trigger, cancellationToken);
    }

    public bool CheckDataProvided()
    {
        throw new InvalidOperationException(
            "This method is not available for this type of job builder. (The job has no data associated with it.)");
    }

    public bool CheckStartScheduled()
    {
        return startScheduled;
    }
}