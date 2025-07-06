using DSDD.Automations.Payments.Persistence.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DSDD.Automations.Payments.Persistence.Cosmos;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="IPayersDao"/> and <see cref="IUnpairedBankPaymentsDao"/> that store data in CosmosDB.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCosmosPaymentsDaos(this IServiceCollection services)
    {
        services.AddOptionsWithValidateOnStart<CosmosOptions>().BindConfiguration("");

        services
            .AddSingleton<IPayersDao, CosmosPayersDao>()
            .AddSingleton<IUnpairedBankPaymentsDao, CosmosUnpairedBankPaymentsDao>();

        return services;
    }
}