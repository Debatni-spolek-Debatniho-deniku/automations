using Microsoft.Extensions.DependencyInjection;

namespace DSDD.Automations.Payments.RBCZ.PremiumApi;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiClient(this IServiceCollection services)
    {
        services.AddOptionsWithValidateOnStart<RbczApiClientOptions>().BindConfiguration("");

#if DEBUG
        services.AddSingleton<IApiClient, SandboxApiClient>();
#else
        services.AddSingleton<IApiClient, ProductionPremiumApiClient>();
#endif

        return services;
    }
}