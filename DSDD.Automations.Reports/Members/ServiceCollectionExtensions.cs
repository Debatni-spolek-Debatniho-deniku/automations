using Azure.Core;
using DSDD.Automations.Payments;
using Microsoft.Extensions.DependencyInjection;

namespace DSDD.Automations.Reports.Members;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMembers(this IServiceCollection services)
    {
        services.AddPnPCore(o =>
        {
            // HACK: PnP core does not allow injection for auth provider. Expected that previous code registered instance of the TokenCredential.
            TokenCredential? credential = services
                .LastOrDefault(d => d.ServiceType == typeof(TokenCredential))?
                .ImplementationInstance as TokenCredential;

            if (credential is null)
                throw new InvalidOperationException($"Could not found instance for service {nameof(TokenCredential)}!");

            o.DefaultAuthenticationProvider = new TokenCredentialAuthenticationProvider(credential);
        });

        services.AddOptionsWithValidateOnStart<SharePointMembersProviderOptions>().BindConfiguration("");

        services.AddTransient<IMembersProvider, SharePointMembersProvider>();
        
        return services;
    }
}