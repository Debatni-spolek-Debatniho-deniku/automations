using System.Security.Cryptography.X509Certificates;
using Azure.Core;
using DSDD.Automations.Reports.Members.Extractor;
using DSDD.Automations.Reports.Members.PnP;
using Microsoft.Extensions.DependencyInjection;
using PnP.Core.Auth;
using PnP.Core.Services.Builder.Configuration;

namespace DSDD.Automations.Reports.Members;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMembers(this IServiceCollection services)
    {
        services.AddPnPCore();
        services
            .AddOptions<PnPCoreOptions>()
            .Configure<TokenCredential>((options, credential) =>
            {
#if DEBUG
                // In DEBUG EnviromentalCredential is used. These env variables are used by it, yet they are not accessible from the object.
                options.DefaultAuthenticationProvider = new X509CertificateAuthenticationProvider(
                    Environment.GetEnvironmentVariable("AZURE_CLIENT_ID"),
                    Environment.GetEnvironmentVariable("AZURE_TENANT_ID"),
                    StoreName.My,
                    StoreLocation.CurrentUser,
                    Environment.GetEnvironmentVariable("DEV_CERTIFICATE_THUMBPRINT"));
#else
            o.DefaultAuthenticationProvider = new TokenCredentialAuthenticationProvider(credential);
#endif
            });

        services.AddOptionsWithValidateOnStart<SharePointMembersProviderOptions>().BindConfiguration("");
        services.AddTransient<IMembersProvider, SharePointMembersProvider>();

        services.AddOptionsWithValidateOnStart<ClosedXMLMembersExtractorOptions>().BindConfiguration("");
        services.AddTransient<IMembersExtractor, ClosedXMLMembersExtractor>();

        return services;
    }
}