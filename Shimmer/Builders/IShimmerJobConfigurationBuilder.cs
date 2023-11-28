using Quartz;
using Shimmer.Core;

namespace Shimmer.Builders;

public interface IShimmerJobConfigurationBuilder<T, TData, TBuilder> where T : ShimmerJob<TData> where TData : class
{
    public JobBuilder JobBuilder { get; set; }
    public TriggerBuilder TriggerBuilder { get; set; }

    public JobKey JobKey { get; set; }

    TBuilder Data(TData data);

    TBuilder Group(string group);

    TBuilder Name(string name);

    TBuilder Description(string description);

    TBuilder Concurrent(bool concurrent = true);

    TBuilder RequestRecovery(bool request = true);

    TBuilder Durable(bool store = true);

    TBuilder Priority(int priority);

    TBuilder Schedule(IScheduleBuilder scheduleBuilder);

    TBuilder SimpleSchedule(Action<SimpleScheduleBuilder> configure);

    TBuilder CronSchedule(string cronExpression);

    TBuilder StartAt(DateTimeOffset startTimeOffset);

    TBuilder EndAt(DateTimeOffset? endTimeOffset);

    bool CheckDataProvided();

    bool CheckStartScheduled();
}

public interface IShimmerJobConfigurationBuilder<T, TBuilder> where T : ShimmerJob
{
    public JobBuilder JobBuilder { get; set; }
    public TriggerBuilder TriggerBuilder { get; set; }

    public JobKey JobKey { get; set; }

    TBuilder Group(string group);

    TBuilder Name(string name);

    TBuilder Description(string description);

    TBuilder Concurrent(bool concurrent = true);

    TBuilder RequestRecovery(bool request = true);

    TBuilder Durable(bool store = true);

    TBuilder Priority(int priority);

    TBuilder Schedule(IScheduleBuilder scheduleBuilder);

    TBuilder SimpleSchedule(Action<SimpleScheduleBuilder> configure);

    TBuilder CronSchedule(string cronExpression);

    TBuilder StartAt(DateTimeOffset startTimeOffset);

    TBuilder EndAt(DateTimeOffset? endTimeOffset);

    bool CheckDataProvided();

    bool CheckStartScheduled();
}