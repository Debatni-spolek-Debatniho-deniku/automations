using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DSDD.Automations.Payments.Helpers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNumbericSymbolParser(this IServiceCollection services)
    {
        services.TryAddSingleton<INumericSymbolParser, NumericSymbolParser>();
        return services;
    }
}