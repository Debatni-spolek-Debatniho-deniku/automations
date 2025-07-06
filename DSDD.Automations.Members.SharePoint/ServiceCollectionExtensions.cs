using System.Security.Cryptography.X509Certificates;
using Azure.Core;
using DSDD.Automations.Members.SharePoint;
using DSDD.Automations.Payments.Helpers;
using DSDD.Automations.Reports.Members.Extractor;
using Microsoft.Extensions.DependencyInjection;
using PnP.Core.Auth;
using PnP.Core.Services.Builder.Configuration;

namespace DSDD.Automations.Reports.Members;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds implementations of <see cref="IMembersProvider"/> that loads members from SharePoint Excel document. 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddSharePointMembersProvider(this IServiceCollection services)
    {
        services.AddNumbericSymbolParser();

        services.AddPnPCore();
        services
            .AddOptions<PnPCoreOptions>()
            .Configure<TokenCredential>((options, credential) =>
            {
#if DEBUG
                // In DEBUG EnviromentalCredential (inhr. TokenCredential) is used.
                // These env variables are used by it, yet they are not accessible from the TokenCredential object.
                options.DefaultAuthenticationProvider = new X509CertificateAuthenticationProvider(
                    Environment.GetEnvironmentVariable("AZURE_CLIENT_ID"),
                    Environment.GetEnvironmentVariable("AZURE_TENANT_ID"),
                    StoreName.My,
                    StoreLocation.CurrentUser,
                    Environment.GetEnvironmentVariable("DEV_CERTIFICATE_THUMBPRINT"));
#else
                options.DefaultAuthenticationProvider = new DSDD.Automations.Members.SharePoint.Auth.TokenCredentialAuthenticationProvider(credential);
#endif
            });

        services.AddOptionsWithValidateOnStart<SharePointMembersProviderOptions>().BindConfiguration("");
        services.AddTransient<IMembersProvider, SharePointMembersProvider>();

        services.AddOptionsWithValidateOnStart<ClosedXMLMembersExtractorOptions>().BindConfiguration("");
        services.AddTransient<IMembersExtractor, ClosedXMLMembersExtractor>();
        services.AddTransient<IMemberFeePeriodParser, MemberFeePeriodParser>();

        return services;
    }
}