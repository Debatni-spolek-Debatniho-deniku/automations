using Azure.Core;
using DSDD.Automations.Payments.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DSDD.Automations.Payments;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPaymentsCommon(this IServiceCollection services)
    {
        services.AddOptionsWithValidateOnStart<CosmosOptions>().BindConfiguration("");

        services
            .AddSingleton<IPayersDao, CosmosPayersDao>()
            .AddTransient<INumericSymbolParser, NumericSymbolParser>();

        return services;
    }
}