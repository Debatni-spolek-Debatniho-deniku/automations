using System.Reflection;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RazorLight;
using RazorLight.Extensions;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddRazorLight(() => new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(Assembly.GetExecutingAssembly(), "DSDD.Automations.Payments.Views")
            .UseMemoryCachingProvider()
            .EnableDebugMode()
            .Build());
    })
    .Build();

host.Run();
