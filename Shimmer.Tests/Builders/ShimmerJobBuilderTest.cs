using Moq;
using Quartz;
using Shimmer.Core;
using Shimmer.Services;

namespace Shimmer.Tests.Builders;

public class JobBuilderTestJob1 : ShimmerJob
{
    protected override Task Process(IJobExecutionContext context)
    {
        throw new NotImplementedException();
    }
}

public class JobBuilderTestJob2 : ShimmerJob<JobBuilderTestData>
{
    protected override Task Process(JobBuilderTestData data, IJobExecutionContext context)
    {
        throw new NotImplementedException();
    }
}

public class JobBuilderTestData;

public class ShimmerJobBuilderTest
{
    [Fact]
    public async Task CreatesSimpleJob()
    {
        var mockScheduler = new Mock<IScheduler>();

        var mockSchedulerFactory = new Mock<ISchedulerFactory>();
        mockSchedulerFactory.Setup(f => f.GetScheduler(default)).ReturnsAsync(mockScheduler.Object);

        var shimmerJobFactory = new ShimmerJobFactory(new ShimmerJobManager(), mockSchedulerFactory.Object);

        var job1 = await shimmerJobFactory.CreateAsync<JobBuilderTestJob1>();

        await job1.FireAsync();

        mockScheduler.Verify(
            e => e.ScheduleJob(It.IsAny<IJobDetail>(), It.IsAny<ITrigger>(), It.IsAny<CancellationToken>()),
            Times.Once);

        var job2 = await shimmerJobFactory.CreateAsync<JobBuilderTestJob2, JobBuilderTestData>();

        job2.Data(new JobBuilderTestData());

        await job2.FireAsync();

        mockScheduler.Verify(
            e => e.ScheduleJob(It.IsAny<IJobDetail>(), It.IsAny<ITrigger>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    [Fact]
    public async Task CreatesJobWithData()
    {
        var mockScheduler = new Mock<IScheduler>();

        var mockSchedulerFactory = new Mock<ISchedulerFactory>();
        mockSchedulerFactory.Setup(f => f.GetScheduler(default)).ReturnsAsync(mockScheduler.Object);

        var shimmerJobFactory = new ShimmerJobFactory(new ShimmerJobManager(), mockSchedulerFactory.Object);

        var job1 = await shimmerJobFactory.CreateAsync<JobBuilderTestJob2, JobBuilderTestData>();

        job1.Data(new JobBuilderTestData());

        await job1.FireAsync();

        mockScheduler.Verify(
            e => e.ScheduleJob(It.IsAny<IJobDetail>(), It.IsAny<ITrigger>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}