using Quartz;
using Shimmer.Core;

namespace Shimmer.Builders;

/// <summary>
///     This interface is used to configure a job. Basically it's the same as Quartz's JobBuilder and TriggerBuilder in one
///     package.
/// </summary>
/// <typeparam name="T">The type of the job.</typeparam>
/// <typeparam name="TData">The type of the job data.</typeparam>
/// <typeparam name="TBuilder">The type of the builder which the methods return.</typeparam>
public interface IShimmerJobConfigurationBuilder<T, TData, TBuilder> where T : ShimmerJob<TData> where TData : class
{
    public JobBuilder JobBuilder { get; set; }
    public TriggerBuilder TriggerBuilder { get; set; }

    public JobKey JobKey { get; set; }

    /// <summary>
    ///     Sets the data of the job.
    /// </summary>
    /// <param name="data">The instance of the data class.</param>
    /// <returns>The job builder.</returns>
    TBuilder Data(TData data);

    /// <summary>
    ///     Sets the Quartz group of the job (in the JobKey).
    /// </summary>
    /// <param name="group">The group name.</param>
    /// <returns>The job builder.</returns>
    TBuilder Group(string group);

    /// <summary>
    ///     Sets the Quartz name of the job (in the JobKey).
    /// </summary>
    /// <param name="name">The job name.</param>
    /// <returns>The job builder.</returns>
    TBuilder Name(string name);

    /// <summary>
    ///     Sets the humanly readable description of the job.
    /// </summary>
    /// <param name="description">The job description.</param>
    /// <returns>The job builder.</returns>
    TBuilder Description(string description);

    /// <summary>
    ///     Sets whether the job can be run concurrently. By default it's true.
    /// </summary>
    /// <param name="concurrent">Whether the job can be run concurrently or not.</param>
    /// <returns>The job builder.</returns>
    TBuilder Concurrent(bool concurrent = true);

    /// <summary>
    ///     A wrapper for the Quartz's RequestRecovery method. By default it's false.
    /// </summary>
    /// <param name="request">Whether it should be recovered or not.</param>
    /// <returns>The job builder.</returns>
    TBuilder RequestRecovery(bool request = true);

    /// <summary>
    ///     A wrapper for the Quartz's StoreDurably method. By default it's false.
    /// </summary>
    /// <param name="store">Whether the job should be stored durably or not.</param>
    /// <returns>The job builder.</returns>
    TBuilder Durable(bool store = true);

    /// <summary>
    ///     Sets the job priority. Higher priority jobs get executed first (in case they're supposed to be executed at the same
    ///     time).
    /// </summary>
    /// <param name="priority">The job priority.</param>
    /// <returns>The job builder</returns>
    TBuilder Priority(int priority);

    /// <summary>
    ///     Schedules the job with the given schedule builder.
    /// </summary>
    /// <param name="scheduleBuilder">The schedule builder.</param>
    /// <returns>The job builder.</returns>
    TBuilder Schedule(IScheduleBuilder scheduleBuilder);

    /// <summary>
    ///     Schedules the job with the given simple schedule builder using an action.
    /// </summary>
    /// <param name="configure">The action to configure the simple schedule.</param>
    /// <returns>The job builder.</returns>
    TBuilder SimpleSchedule(Action<SimpleScheduleBuilder> configure);

    /// <summary>
    ///     Schedules the job with the given cron expression.
    /// </summary>
    /// <param name="cronExpression">The cron expression string.</param>
    /// <returns>The job builder.</returns>
    TBuilder CronSchedule(string cronExpression);

    /// <summary>
    ///     Sets when the job should start executing.
    /// </summary>
    /// <param name="startTimeOffset">The start time.</param>
    /// <returns>The job builder.</returns>
    TBuilder StartAt(DateTimeOffset startTimeOffset);

    /// <summary>
    ///     Sets when the job should stop executing (even if it has repeats remaining).
    /// </summary>
    /// <param name="endTimeOffset">The stop time.</param>
    /// <returns>The job builder.</returns>
    TBuilder EndAt(DateTimeOffset? endTimeOffset);

    bool CheckDataProvided();

    bool CheckStartScheduled();
}

/// <summary>
///     This interface is used to configure a job. Basically it's the same as Quartz's JobBuilder and TriggerBuilder in one
///     package.
/// </summary>
/// <typeparam name="T">The type of the job.</typeparam>
/// <typeparam name="TBuilder">The type of the builder which the methods return.</typeparam>
public interface IShimmerJobConfigurationBuilder<T, TBuilder> where T : ShimmerJob
{
    public JobBuilder JobBuilder { get; set; }
    public TriggerBuilder TriggerBuilder { get; set; }

    public JobKey JobKey { get; set; }

    /// <summary>
    ///     Sets the Quartz group of the job (in the JobKey).
    /// </summary>
    /// <param name="group">The group name.</param>
    /// <returns>The job builder.</returns>
    TBuilder Group(string group);

    /// <summary>
    ///     Sets the Quartz name of the job (in the JobKey).
    /// </summary>
    /// <param name="name">The job name.</param>
    /// <returns>The job builder.</returns>
    TBuilder Name(string name);

    /// <summary>
    ///     Sets the humanly readable description of the job.
    /// </summary>
    /// <param name="description">The job description.</param>
    /// <returns>The job builder.</returns>
    TBuilder Description(string description);

    /// <summary>
    ///     Sets whether the job can be run concurrently. By default it's true.
    /// </summary>
    /// <param name="concurrent">Whether the job can be run concurrently or not.</param>
    /// <returns>The job builder.</returns>
    TBuilder Concurrent(bool concurrent = true);

    /// <summary>
    ///     A wrapper for the Quartz's RequestRecovery method. By default it's false.
    /// </summary>
    /// <param name="request">Whether it should be recovered or not.</param>
    /// <returns>The job builder.</returns>
    TBuilder RequestRecovery(bool request = true);

    /// <summary>
    ///     A wrapper for the Quartz's StoreDurably method. By default it's false.
    /// </summary>
    /// <param name="store">Whether the job should be stored durably or not.</param>
    /// <returns>The job builder.</returns>
    TBuilder Durable(bool store = true);

    /// <summary>
    ///     Sets the job priority. Higher priority jobs get executed first (in case they're supposed to be executed at the same
    ///     time).
    /// </summary>
    /// <param name="priority">The job priority.</param>
    /// <returns>The job builder</returns>
    TBuilder Priority(int priority);

    /// <summary>
    ///     Schedules the job with the given schedule builder.
    /// </summary>
    /// <param name="scheduleBuilder">The schedule builder.</param>
    /// <returns>The job builder.</returns>
    TBuilder Schedule(IScheduleBuilder scheduleBuilder);

    /// <summary>
    ///     Schedules the job with the given simple schedule builder using an action.
    /// </summary>
    /// <param name="configure">The action to configure the simple schedule.</param>
    /// <returns>The job builder.</returns>
    TBuilder SimpleSchedule(Action<SimpleScheduleBuilder> configure);

    /// <summary>
    ///     Schedules the job with the given cron expression.
    /// </summary>
    /// <param name="cronExpression">The cron expression string.</param>
    /// <returns>The job builder.</returns>
    TBuilder CronSchedule(string cronExpression);

    /// <summary>
    ///     Sets when the job should start executing.
    /// </summary>
    /// <param name="startTimeOffset">The start time.</param>
    /// <returns>The job builder.</returns>
    TBuilder StartAt(DateTimeOffset startTimeOffset);

    /// <summary>
    ///     Sets when the job should stop executing (even if it has repeats remaining).
    /// </summary>
    /// <param name="endTimeOffset">The stop time.</param>
    /// <returns>The job builder.</returns>
    TBuilder EndAt(DateTimeOffset? endTimeOffset);

    bool CheckDataProvided();

    bool CheckStartScheduled();
}