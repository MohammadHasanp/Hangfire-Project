namespace HangfireProject.Core.Hangfire;

public interface IJobBaseService
{
    public string CronExpression { get; }
    public string GetQueueName();
    public Task Execute(CancellationToken cancellationToken = default);
    public Task Register();
}
