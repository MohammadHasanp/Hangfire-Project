using Microsoft.Extensions.DependencyInjection;

namespace HangfireProject.Core.Common;

public static class ServiceLocator
{
    public static IServiceProvider Instance { get; set; } = null!;

    public static void Initialize(IServiceProvider serviceProvider)
    {
        Instance = serviceProvider;
    }

    public static IEnumerable<T> GetServices<T>() where T : class
    {
        return Instance.GetServices<T>();
    }

    public static T? GetService<T>() where T : class => Instance.GetService<T>();
}
