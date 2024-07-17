using Azure.Core;
using DSDD.Automations.Payments.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DSDD.Automations.Payments;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPaymentsCommon(this IServiceCollection services, string cosmsoAccountEndpoint)
        => services
            .AddSingleton<IPayersDao>(sp => new CosmosPayersDao(sp.GetRequiredService<TokenCredential>(), cosmsoAccountEndpoint))
            .AddTransient<INumericSymbolParser, NumericSymbolParser>();
}