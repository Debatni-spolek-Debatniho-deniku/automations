using System.Reflection;
using DSDD.Automations.Hosting;
using DSDD.Automations.Payments;
using DSDD.Automations.Payments.Middleware;
using DSDD.Automations.Payments.Payments;
using DSDD.Automations.Payments.RBCZ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RazorLight;
using RazorLight.Extensions;

var host = new HostBuilder()
    .ConfigureAutomationsFunctionsWebApplication(app =>
    {
        app.UseMiddleware<PaymentsAdministratorAuthorizationMiddleware>();
        app.UseMiddleware<ErrorPageMiddleware>();
    })
    .ConfigureServices((ctx, services) =>
    {
        services.AddRazorLight(() => new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(Assembly.GetExecutingAssembly(), "DSDD.Automations.Payments.Views")
            .UseMemoryCachingProvider()
            .EnableDebugMode()
            .Build());
        
        services.AddTransient<IPaymentsService, PayerPaymentsService>();

        services.AddPaymentsCommon();
        services.AddPaymentsRBCZ();
    })
    .Build();

host.Run();
