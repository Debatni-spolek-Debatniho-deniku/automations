using Microsoft.Extensions.DependencyInjection;
using DSDD.Automations.Reports.Members;

namespace DSDD.Automations.Members.Http;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHttpMembersProvider(this IServiceCollection services)
    {
        services.AddOptionsWithValidateOnStart<HttpMembersProviderOptions>().BindConfiguration("");
        services.AddHttpClient<HttpMembersProvider>();
        services.AddTransient<IMembersProvider, HttpMembersProvider>();
        return services;
    }
}
