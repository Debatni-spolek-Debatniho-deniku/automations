using Microsoft.Extensions.DependencyInjection;

namespace DSDD.Automations.Mailing;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMailing(this IServiceCollection services)
    {
        services.AddOptionsWithValidateOnStart<MailingOptions>().BindConfiguration("");
        services.AddScoped<IMailingService, MailKitMailingService>();

        return services;
    }
}