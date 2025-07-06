using Microsoft.Extensions.DependencyInjection;

namespace DSDD.Automations.Payments.Banking.RBCZ.PremiumApi;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiClient(this IServiceCollection services)
    {
        services.AddOptionsWithValidateOnStart<PremiumApiClientOptions>().BindConfiguration("");
        services.AddSingleton<IPremiumApiClient, PremiumApiClient>();

        return services;
    }
}