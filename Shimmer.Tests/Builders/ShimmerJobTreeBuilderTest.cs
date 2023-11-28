using FluentAssertions;
using Moq;
using Quartz;
using Shimmer.Core;
using Shimmer.Services;

namespace Shimmer.Tests.Builders;

public class JobTreeBuilderTestJob1 : ShimmerJob
{
    protected override Task Process(IJobExecutionContext context)
    {
        throw new NotImplementedException();
    }
}

public class JobTreeBuilderTestJob2 : ShimmerJob
{
    protected override Task Process(IJobExecutionContext context)
    {
        throw new NotImplementedException();
    }
}

public class JobTreeBuilderTestJob3 : ShimmerJob<JobTreeBuilderTestData>
{
    protected override Task Process(JobTreeBuilderTestData data, IJobExecutionContext context)
    {
        throw new NotImplementedException();
    }
}

public class JobTreeBuilderTestData;

public class ShimmerJobTreeBuilderTest
{
    [Fact]
    public async Task CreatesSimpleJob()
    {
        var mockScheduler = new Mock<IScheduler>();

        var mockSchedulerFactory = new Mock<ISchedulerFactory>();
        mockSchedulerFactory.Setup(f => f.GetScheduler(default)).ReturnsAsync(mockScheduler.Object);

        var shimmerJobFactory = new ShimmerJobFactory(new ShimmerJobManager(), mockSchedulerFactory.Object);

        var job1 = await shimmerJobFactory.CreateTreeAsync<JobTreeBuilderTestJob1>();

        await job1.FireAsync();

        mockScheduler.Verify(
            e => e.ScheduleJob(It.IsAny<IJobDetail>(), It.IsAny<ITrigger>(), It.IsAny<CancellationToken>()),
            Times.Once);

        var job2 = await shimmerJobFactory.CreateTreeAsync<JobBuilderTestJob1>();

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

        var job1 = await shimmerJobFactory.CreateTreeAsync<JobTreeBuilderTestJob3, JobTreeBuilderTestData>();

        var fireAction = async () => await job1.FireAsync();

        await fireAction.Should().ThrowAsync<InvalidOperationException>();

        job1.Data(new JobTreeBuilderTestData());

        await fireAction.Should().NotThrowAsync<InvalidOperationException>();

        mockScheduler.Verify(
            e => e.ScheduleJob(It.IsAny<IJobDetail>(), It.IsAny<ITrigger>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreatesCorrectJobTree()
    {
        var mockScheduler = new Mock<IScheduler>();

        var mockSchedulerFactory = new Mock<ISchedulerFactory>();
        mockSchedulerFactory.Setup(f => f.GetScheduler(default)).ReturnsAsync(mockScheduler.Object);

        var jobManager = new ShimmerJobManager();
        var shimmerJobFactory = new ShimmerJobFactory(jobManager, mockSchedulerFactory.Object);

        var job1 = await shimmerJobFactory.CreateTreeAsync<JobTreeBuilderTestJob1>();

        job1.AddDependentJob<JobTreeBuilderTestJob3, JobTreeBuilderTestData>(j =>
            j.Data(new JobTreeBuilderTestData()));

        var job2 = await shimmerJobFactory.CreateTreeAsync<JobTreeBuilderTestJob2>();

        job1.AddDependentJob(job2);

        await job1.FireAsync();

        mockScheduler.Verify(
            e => e.ScheduleJob(It.IsAny<IJobDetail>(), It.IsAny<ITrigger>(), It.IsAny<CancellationToken>()),
            Times.Once);

        jobManager.JobTreeMap.Should().HaveCount(1);
        jobManager.JobTreeMap.First().Value.Should().HaveCount(2);

        jobManager.JobTreeMap.First().Value.JobDetail.Job.Key.Should().Be(job1.JobKey);
        jobManager.JobTreeMap.First().Key.Should().Be(job1.JobKey);

        var job3 = await shimmerJobFactory.CreateTreeAsync<JobTreeBuilderTestJob3, JobTreeBuilderTestData>();

        job3.Data(new JobTreeBuilderTestData())
            .AddDependentJob<JobTreeBuilderTestJob1>(j => j.AddDependentJob<JobTreeBuilderTestJob2>(_ => { }));

        await job3.FireAsync();

        jobManager.JobTreeMap.Should().HaveCount(2);
        jobManager.JobTreeMap[job3.JobKey].Should().HaveCount(1);
        jobManager.JobTreeMap[job3.JobKey].First().Should().HaveCount(1);
    }
}