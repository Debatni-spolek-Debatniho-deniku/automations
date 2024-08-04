using Azure.Core;
using Azure.Identity;
using DSDD.Automations.Hosting.Middleware;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DSDD.Automations.Hosting;

public static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureAutomationsFunctionsWebApplication(
        this IHostBuilder hostBuilder,
        Action<IFunctionsWorkerApplicationBuilder>? configure = null)
        => hostBuilder.ConfigureFunctionsWebApplication(app =>
        {
            app.UseMiddleware<ClaimsPrincipalPoplulatingMiddleware>();

            configure?.Invoke(app);

            app.Services.AddApplicationInsightsTelemetryWorkerService();
            app.Services.ConfigureFunctionsApplicationInsights();

#if DEBUG
            app.Services.AddSingleton<TokenCredential>(new EnvironmentCredential());
#else
            app.Services.AddSingleton<TokenCredential>(new ManagedIdentityCredential());
#endif
        });
}