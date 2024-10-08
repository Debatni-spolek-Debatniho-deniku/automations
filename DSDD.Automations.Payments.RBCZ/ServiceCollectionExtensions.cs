﻿using DSDD.Automations.Payments.RBCZ.Certificates;
using DSDD.Automations.Payments.RBCZ.PremiumApi;
using Microsoft.Extensions.DependencyInjection;

namespace DSDD.Automations.Payments.RBCZ;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPaymentsRBCZ(this IServiceCollection services)
        => services
            .AddApiClient()
            .AddAccountCertificateProvider()
            .AddSingleton<IFxRates, FxRates>()
            .AddTransient<IBankPaymentsImporter, BankPaymentsImporter>();
}