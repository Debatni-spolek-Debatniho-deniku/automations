using Azure.Core;
using DSDD.Automations.Payments.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DSDD.Automations.Payments;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPaymentsCommon(this IServiceCollection services, string cosmsoAccountEndpoint)
        => services
            .AddSingleton<IPayers>(sp =>
                new CosmosPayers(sp.GetRequiredService<TokenCredential>(), cosmsoAccountEndpoint))
            .AddSingleton<INumericSymbolParser, NumericSymbolParser>();
}