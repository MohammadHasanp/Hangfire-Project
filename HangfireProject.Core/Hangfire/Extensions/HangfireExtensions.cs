using System.Reflection;
using HangfireProject.Core.Common;
using Microsoft.Extensions.DependencyInjection;

namespace HangfireProject.Core.Hangfire.Extensions;

public static class HangfireExtensions
{
    public static IServiceCollection AddJob(this IServiceCollection services)
    {
        var assembly = Assembly.GetAssembly(typeof(HangfireExtensions));
        var jobs = assembly!.GetTypes()
            .Where(t => typeof(IJobBaseService).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var item in jobs)
        {
            services.AddTransient(typeof(IJobBaseService), item);
        }
        return services;
    }

    public static async Task RegisterJob()
    {
        var jobs = ServiceLocator.GetServices<IJobBaseService>();
        foreach (var item in jobs)
        {
            await item.Register();
        }
    }
}
