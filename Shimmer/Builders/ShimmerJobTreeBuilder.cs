using System.Text.Json;
using Quartz;
using Shimmer.Core;
using Shimmer.Services;

namespace Shimmer.Builders;

/// <summary>
///     The builder used to configure and build jobs that depend on each other.
/// </summary>
/// <param name="jobManager">The job manager (same as the one used by the jobs).</param>
/// <param name="scheduler">The Quartz scheduler to use.</param>
/// <typeparam name="T">The type of the job.</typeparam>
/// <typeparam name="TData">The type of the job data.</typeparam>
public class ShimmerJobTreeBuilder<T, TData>
    (IShimmerJobManager jobManager, IScheduler scheduler) : IShimmerJobTreeBuilder<T, TData>
    where T : ShimmerJob<TData> where TData : class
{
    public bool dataProvided;

    public bool startScheduled;
    public ShimmerJobTreeNode? CurrentNode { get; set; }

    public JobBuilder JobBuilder { get; set; } = JobBuilder.Create<T>();
    public TriggerBuilder TriggerBuilder { get; set; } = TriggerBuilder.Create();

    public JobKey JobKey { get; set; } = new(Guid.NewGuid().ToString());

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T, TData> Data(TData data)
    {
        JobBuilder.UsingJobData("data", JsonSerializer.Serialize(data));
        dataProvided = true;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T, TData> Group(string group)
    {
        JobKey.Group = group;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T, TData> Name(string name)
    {
        JobKey.Name = name;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T, TData> Description(string description)
    {
        JobBuilder.WithDescription(description);

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T, TData> Concurrent(bool concurrent = true)
    {
        JobBuilder.DisallowConcurrentExecution(!concurrent);

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T, TData> RequestRecovery(bool request = true)
    {
        JobBuilder.RequestRecovery(request);

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T, TData> Durable(bool store = true)
    {
        JobBuilder.StoreDurably(store);

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T, TData> Priority(int priority)
    {
        TriggerBuilder.WithPriority(priority);

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T, TData> Schedule(IScheduleBuilder scheduleBuilder)
    {
        TriggerBuilder.WithSchedule(scheduleBuilder);
        startScheduled = true;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T, TData> SimpleSchedule(Action<SimpleScheduleBuilder> configure)
    {
        TriggerBuilder.WithSimpleSchedule(configure);
        startScheduled = true;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T, TData> CronSchedule(string cronExpression)
    {
        TriggerBuilder.WithCronSchedule(cronExpression);
        startScheduled = true;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T, TData> StartAt(DateTimeOffset startTimeOffset)
    {
        TriggerBuilder.StartAt(startTimeOffset);
        startScheduled = true;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T, TData> EndAt(DateTimeOffset? endTimeOffset)
    {
        TriggerBuilder.EndAt(endTimeOffset);

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T, TData> AddDependentJob<TNew>(
        Action<IShimmerJobTreeBuilder<TNew>>? configure = null)
        where TNew : ShimmerJob
    {
        JobBuilder.WithIdentity(JobKey);

        if (!startScheduled)
            TriggerBuilder.StartNow();

        var currentJobDetail = new ShimmerJobDetail
        {
            Job = JobBuilder.Build(),
            Trigger = TriggerBuilder.Build()
        };

        var node = new ShimmerJobTreeNode
        {
            JobDetail = currentJobDetail
        };

        CurrentNode ??= node;

        var jobTreeBuilder = new ShimmerJobTreeBuilder<TNew>(jobManager, scheduler);

        configure?.Invoke(jobTreeBuilder);

        jobTreeBuilder.JobBuilder.WithIdentity(jobTreeBuilder.JobKey);

        if (!jobTreeBuilder.startScheduled)
            jobTreeBuilder.TriggerBuilder.StartNow();

        CurrentNode.Add(jobTreeBuilder.CurrentNode ?? new ShimmerJobTreeNode
        {
            JobDetail = new ShimmerJobDetail
            {
                Job = jobTreeBuilder.JobBuilder.Build(),
                Trigger = jobTreeBuilder.TriggerBuilder.Build()
            }
        });

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T, TData> AddDependentJob<TNew, TNewData>(
        Action<IShimmerJobTreeBuilder<TNew, TNewData>> configure)
        where TNew : ShimmerJob<TNewData> where TNewData : class
    {
        JobBuilder.WithIdentity(JobKey);

        if (!startScheduled)
            TriggerBuilder.StartNow();

        var currentJobDetail = new ShimmerJobDetail
        {
            Job = JobBuilder.Build(),
            Trigger = TriggerBuilder.Build()
        };

        var node = new ShimmerJobTreeNode
        {
            JobDetail = currentJobDetail
        };

        CurrentNode ??= node;

        var jobTreeBuilder = new ShimmerJobTreeBuilder<TNew, TNewData>(jobManager, scheduler);

        configure(jobTreeBuilder);

        if (!jobTreeBuilder.CheckDataProvided())
            throw new InvalidOperationException(
                "No data has been provided. Did you forget to call Data() on the passed IJobTreeBuilder?");

        jobTreeBuilder.JobBuilder.WithIdentity(jobTreeBuilder.JobKey);

        if (!jobTreeBuilder.CheckStartScheduled())
            jobTreeBuilder.TriggerBuilder.StartNow();

        CurrentNode.Add(jobTreeBuilder.CurrentNode ?? new ShimmerJobTreeNode
        {
            JobDetail = new ShimmerJobDetail
            {
                Job = jobTreeBuilder.JobBuilder.Build(),
                Trigger = jobTreeBuilder.TriggerBuilder.Build()
            }
        });

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T, TData> AddDependentJob<TNew>(IShimmerJobTreeBuilder<TNew> jobBuilder)
        where TNew : ShimmerJob
    {
        JobBuilder.WithIdentity(JobKey);

        if (!startScheduled)
            TriggerBuilder.StartNow();

        var currentJobDetail = new ShimmerJobDetail
        {
            Job = JobBuilder.Build(),
            Trigger = TriggerBuilder.Build()
        };

        var node = new ShimmerJobTreeNode
        {
            JobDetail = currentJobDetail
        };

        CurrentNode ??= node;

        jobBuilder.JobBuilder.WithIdentity(jobBuilder.JobKey);

        if (!jobBuilder.CheckStartScheduled())
            jobBuilder.TriggerBuilder.StartNow();

        CurrentNode.Add(jobBuilder.CurrentNode ?? new ShimmerJobTreeNode
        {
            JobDetail = new ShimmerJobDetail
            {
                Job = jobBuilder.JobBuilder.Build(),
                Trigger = jobBuilder.TriggerBuilder.Build()
            }
        });

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T, TData>
        AddDependentJob<TNew, TNewData>(IShimmerJobTreeBuilder<TNew, TNewData> jobBuilder)
        where TNew : ShimmerJob<TNewData> where TNewData : class
    {
        JobBuilder.WithIdentity(JobKey);

        if (!startScheduled)
            TriggerBuilder.StartNow();

        var currentJobDetail = new ShimmerJobDetail
        {
            Job = JobBuilder.Build(),
            Trigger = TriggerBuilder.Build()
        };

        var node = new ShimmerJobTreeNode
        {
            JobDetail = currentJobDetail
        };

        CurrentNode ??= node;

        if (!jobBuilder.CheckDataProvided())
            throw new InvalidOperationException(
                "No data has been provided. Did you forget to call Data() on the passed IJobTreeBuilder?");

        jobBuilder.JobBuilder.WithIdentity(jobBuilder.JobKey);

        if (!jobBuilder.CheckStartScheduled())
            jobBuilder.TriggerBuilder.StartNow();

        CurrentNode.Add(jobBuilder.CurrentNode ?? new ShimmerJobTreeNode
        {
            JobDetail = new ShimmerJobDetail
            {
                Job = jobBuilder.JobBuilder.Build(),
                Trigger = jobBuilder.TriggerBuilder.Build()
            }
        });

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

        if (CurrentNode != null)
            jobManager.JobTreeMap[CurrentNode.JobDetail.Job.Key] = CurrentNode;

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
///     The builder used to configure and build jobs that depend on each other.
/// </summary>
/// <param name="jobManager">The job manager (same as the one used by the jobs).</param>
/// <param name="scheduler">The Quartz scheduler to use.</param>
/// <typeparam name="T">The type of the job.</typeparam>
public class ShimmerJobTreeBuilder<T>(IShimmerJobManager jobManager, IScheduler scheduler) : IShimmerJobTreeBuilder<T>
    where T : ShimmerJob
{
    public bool startScheduled;
    public ShimmerJobTreeNode? CurrentNode { get; set; }

    public JobBuilder JobBuilder { get; set; } = JobBuilder.Create<T>();
    public TriggerBuilder TriggerBuilder { get; set; } = TriggerBuilder.Create();

    public JobKey JobKey { get; set; } = new(Guid.NewGuid().ToString());

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T> Group(string group)
    {
        JobKey.Group = group;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T> Name(string name)
    {
        JobKey.Name = name;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T> Description(string description)
    {
        JobBuilder.WithDescription(description);

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T> Concurrent(bool concurrent = true)
    {
        JobBuilder.DisallowConcurrentExecution(!concurrent);

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T> RequestRecovery(bool request = true)
    {
        JobBuilder.RequestRecovery(request);

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T> Durable(bool store = true)
    {
        JobBuilder.StoreDurably(store);

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T> Priority(int priority)
    {
        TriggerBuilder.WithPriority(priority);

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T> Schedule(IScheduleBuilder scheduleBuilder)
    {
        TriggerBuilder.WithSchedule(scheduleBuilder);
        startScheduled = true;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T> SimpleSchedule(Action<SimpleScheduleBuilder> configure)
    {
        TriggerBuilder.WithSimpleSchedule(configure);
        startScheduled = true;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T> CronSchedule(string cronExpression)
    {
        TriggerBuilder.WithCronSchedule(cronExpression);
        startScheduled = true;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T> StartAt(DateTimeOffset startTimeOffset)
    {
        TriggerBuilder.StartAt(startTimeOffset);
        startScheduled = true;

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T> EndAt(DateTimeOffset? endTimeOffset)
    {
        TriggerBuilder.EndAt(endTimeOffset);

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T> AddDependentJob<TNew>(Action<IShimmerJobTreeBuilder<TNew>>? configure = null)
        where TNew : ShimmerJob
    {
        JobBuilder.WithIdentity(JobKey);

        if (!startScheduled)
            TriggerBuilder.StartNow();

        var currentJobDetail = new ShimmerJobDetail
        {
            Job = JobBuilder.Build(),
            Trigger = TriggerBuilder.Build()
        };

        var node = new ShimmerJobTreeNode
        {
            JobDetail = currentJobDetail
        };

        CurrentNode ??= node;

        var jobChainBuilder = new ShimmerJobTreeBuilder<TNew>(jobManager, scheduler);

        configure?.Invoke(jobChainBuilder);

        jobChainBuilder.JobBuilder.WithIdentity(jobChainBuilder.JobKey);

        if (!jobChainBuilder.startScheduled)
            jobChainBuilder.TriggerBuilder.StartNow();

        CurrentNode.Add(jobChainBuilder.CurrentNode ?? new ShimmerJobTreeNode
        {
            JobDetail = new ShimmerJobDetail
            {
                Job = jobChainBuilder.JobBuilder.Build(),
                Trigger = jobChainBuilder.TriggerBuilder.Build()
            }
        });

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T> AddDependentJob<TNew, TNewData>(
        Action<IShimmerJobTreeBuilder<TNew, TNewData>> configure)
        where TNew : ShimmerJob<TNewData> where TNewData : class
    {
        JobBuilder.WithIdentity(JobKey);

        if (!startScheduled)
            TriggerBuilder.StartNow();

        var currentJobDetail = new ShimmerJobDetail
        {
            Job = JobBuilder.Build(),
            Trigger = TriggerBuilder.Build()
        };

        var node = new ShimmerJobTreeNode
        {
            JobDetail = currentJobDetail
        };

        CurrentNode ??= node;

        var jobChainBuilder = new ShimmerJobTreeBuilder<TNew, TNewData>(jobManager, scheduler);

        configure(jobChainBuilder);

        if (!jobChainBuilder.dataProvided)
            throw new InvalidOperationException(
                "No data has been provided. Did you forget to call Data() on the passed IJobTreeBuilder?");

        jobChainBuilder.JobBuilder.WithIdentity(jobChainBuilder.JobKey);

        if (!jobChainBuilder.startScheduled)
            jobChainBuilder.TriggerBuilder.StartNow();

        CurrentNode.Add(jobChainBuilder.CurrentNode ?? new ShimmerJobTreeNode
        {
            JobDetail = new ShimmerJobDetail
            {
                Job = jobChainBuilder.JobBuilder.Build(),
                Trigger = jobChainBuilder.TriggerBuilder.Build()
            }
        });

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T> AddDependentJob<TNew>(IShimmerJobTreeBuilder<TNew> jobBuilder)
        where TNew : ShimmerJob
    {
        JobBuilder.WithIdentity(JobKey);

        if (!startScheduled)
            TriggerBuilder.StartNow();

        var currentJobDetail = new ShimmerJobDetail
        {
            Job = JobBuilder.Build(),
            Trigger = TriggerBuilder.Build()
        };

        var node = new ShimmerJobTreeNode
        {
            JobDetail = currentJobDetail
        };

        CurrentNode ??= node;

        jobBuilder.JobBuilder.WithIdentity(jobBuilder.JobKey);

        if (!jobBuilder.CheckStartScheduled())
            jobBuilder.TriggerBuilder.StartNow();

        CurrentNode.Add(jobBuilder.CurrentNode ?? new ShimmerJobTreeNode
        {
            JobDetail = new ShimmerJobDetail
            {
                Job = jobBuilder.JobBuilder.Build(),
                Trigger = jobBuilder.TriggerBuilder.Build()
            }
        });

        return this;
    }

    /// <inheritdoc />
    public IShimmerJobTreeBuilder<T>
        AddDependentJob<TNew, TNewData>(IShimmerJobTreeBuilder<TNew, TNewData> jobBuilder)
        where TNew : ShimmerJob<TNewData> where TNewData : class
    {
        JobBuilder.WithIdentity(JobKey);

        if (!startScheduled)
            TriggerBuilder.StartNow();

        var currentJobDetail = new ShimmerJobDetail
        {
            Job = JobBuilder.Build(),
            Trigger = TriggerBuilder.Build()
        };

        var node = new ShimmerJobTreeNode
        {
            JobDetail = currentJobDetail
        };

        CurrentNode ??= node;

        if (!jobBuilder.CheckDataProvided())
            throw new InvalidOperationException(
                "No data has been provided. Did you forget to call Data() on the passed IJobTreeBuilder?");

        jobBuilder.JobBuilder.WithIdentity(jobBuilder.JobKey);

        if (!jobBuilder.CheckStartScheduled())
            jobBuilder.TriggerBuilder.StartNow();

        CurrentNode.Add(jobBuilder.CurrentNode ?? new ShimmerJobTreeNode
        {
            JobDetail = new ShimmerJobDetail
            {
                Job = jobBuilder.JobBuilder.Build(),
                Trigger = jobBuilder.TriggerBuilder.Build()
            }
        });

        return this;
    }

    /// <inheritdoc />
    public Task<DateTimeOffset> FireAsync(CancellationToken cancellationToken = default)
    {
        JobBuilder.WithIdentity(JobKey);

        if (!CheckStartScheduled())
            TriggerBuilder.StartNow();

        var job = JobBuilder.Build();
        var trigger = TriggerBuilder.Build();

        if (CurrentNode != null)
            jobManager.JobTreeMap[CurrentNode.JobDetail.Job.Key] = CurrentNode;

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