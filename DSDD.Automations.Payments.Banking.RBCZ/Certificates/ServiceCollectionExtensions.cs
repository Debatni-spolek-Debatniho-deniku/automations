using Microsoft.Extensions.DependencyInjection;

namespace DSDD.Automations.Payments.RBCZ.Certificates;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAccountCertificateProvider(this IServiceCollection services)
    {
        services.AddOptionsWithValidateOnStart<BlobStorageAccountCertificateOptions>().BindConfiguration("");
        services.AddTransient<IAccountCertificateProvider, BlobStorageAccountCertificateProvider>();

        return services;
    }
}