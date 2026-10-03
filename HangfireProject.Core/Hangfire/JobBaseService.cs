using Hangfire;
using HangfireProject.Core.Common;

namespace HangfireProject.Core.Hangfire;

public abstract class JobBaseService : IJobBaseService
{
    public abstract string CronExpression { get; }

    public virtual string GetQueueName() => nameof(EnHangfireQueue.defaults);

    public abstract Task Execute(CancellationToken cancellationToken = default);
    public virtual Task Register()
    {
        var jobName = $"{GetType().Name}";

        RecurringJob.AddOrUpdate(
            recurringJobId: jobName,
            queue: GetQueueName(),
            methodCall: () => Execute(CancellationToken.None),
            cronExpression: CronExpression,
            new RecurringJobOptions()
            {
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time"),
            });
        return Task.CompletedTask;
    }
}