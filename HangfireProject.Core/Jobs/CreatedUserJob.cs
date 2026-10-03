using Hangfire;
using HangfireProject.Core.Common;
using HangfireProject.Core.Hangfire;
using HangfireProject.Core.Repository;
using HangfireProject.Data.Entities;

namespace HangfireProject.Core.Jobs;

public class CreatedUserJob(IUserRepository repository) : JobBaseService
{
    public override string CronExpression { get; } = Cron.Minutely();
    public override string GetQueueName() => nameof(EnHangfireQueue.created);

    public override async Task Execute(CancellationToken cancellationToken = default)
    {
        await repository.Create(new User
        {
            Family = "TestFamily",
            Name = "TestName",
            Password = "123456789"
        });
    }
}