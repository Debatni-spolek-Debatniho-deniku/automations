using System.Reflection;
using Azure.Core;
using Azure.Identity;
using DSDD.Automations.Payments;
using DSDD.Automations.Payments.RBCZ;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RazorLight;
using RazorLight.Extensions;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((ctx, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddRazorLight(() => new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(Assembly.GetExecutingAssembly(), "DSDD.Automations.Payments.Views")
            .UseMemoryCachingProvider()
            .EnableDebugMode()
            .Build());
        
        services.AddSingleton<TokenCredential>(new DefaultAzureCredential(new DefaultAzureCredentialOptions()));
        services.AddPaymentsCommon(ctx.Configuration.GetValue<string>("COSMOS_DB_ACCOUNT_ENDPOINT") ??
                                 throw new NullReferenceException("CosmosDB was not configured!"));
        services.AddPaymentsRBCZ();
    })
    .Build();

host.Run();
