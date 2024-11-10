using Microsoft.Extensions.DependencyInjection;

namespace DSDD.Automations.Mailing;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMailingClient(this IServiceCollection services)
    {
        services.AddOptionsWithValidateOnStart<MailingOptions>().BindConfiguration("");
        services.AddSingleton<IMailingService, MailKitMailingService>();

        return services;
    }
}