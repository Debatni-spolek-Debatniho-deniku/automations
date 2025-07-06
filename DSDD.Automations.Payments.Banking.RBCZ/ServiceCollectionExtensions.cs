using DSDD.Automations.Payments.Banking.Abstractions;
using DSDD.Automations.Payments.Banking.RBCZ.PremiumApi;
using DSDD.Automations.Payments.Helpers;
using DSDD.Automations.Payments.RBCZ.Certificates;
using Microsoft.Extensions.DependencyInjection;

namespace DSDD.Automations.Payments.Banking.RBCZ;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRbczImporter(this IServiceCollection services)
        => services
            .AddNumbericSymbolParser()
            .AddApiClient()
            .AddAccountCertificateProvider()
            // TODO: Shouldnt this be scoped?
            .AddSingleton<IFxRates, FxRates>()
            .AddTransient<IBankPaymentsImporter, BankPaymentsImporter>();
}