using Microsoft.Extensions.DependencyInjection;

namespace DSDD.Automations.Hosting.SisterApps;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureSisterApps(this IServiceCollection services)
    {
        services.AddOptionsWithValidateOnStart<SisterAppsOptions>().BindConfiguration("");
        return services;
    }
}