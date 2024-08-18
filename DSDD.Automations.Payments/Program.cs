using System.Reflection;
using DSDD.Automations.Hosting;
using DSDD.Automations.Hosting.Razor;
using DSDD.Automations.Hosting.SisterApps;
using DSDD.Automations.Payments;
using DSDD.Automations.Payments.Middleware;
using DSDD.Automations.Payments.Payments;
using DSDD.Automations.Payments.RBCZ;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureAutomationsFunctionsWebApplication(app =>
    {
        app.UseMiddleware<PaymentsAdministratorAuthorizationMiddleware>();
        app.UseMiddleware<ErrorPageMiddleware>();
    })
    .ConfigureServices((ctx, services) =>
    {
        services.ConfigureSisterApps();

        services.AddRazorRenderer(new CompiledRazorAssemblyPart(Assembly.GetExecutingAssembly()));

        services.AddTransient<IPaymentsService, PayerPaymentsService>();

        services.AddPaymentsCommon();
        services.AddPaymentsRBCZ();
    })
    .Build();

host.Run();
