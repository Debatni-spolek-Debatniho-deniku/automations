using System.Reflection;
using DSDD.Automations.Hosting;
using DSDD.Automations.Hosting.SisterApps;
using DSDD.Automations.Payments;
using DSDD.Automations.Reports.Members;
using DSDD.Automations.Reports.Middleware;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using DSDD.Automations.Reports.Razor;
using DSDD.Automations.Reports.Reports;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

var host = new HostBuilder()
    .ConfigureAutomationsFunctionsWebApplication(app =>
    {
        app.UseMiddleware<ReportsReaderAuthorizationMiddleware>();
        app.UseMiddleware<ErrorPageMiddleware>();
    })
    .ConfigureServices(services =>
    {
        services.ConfigureSisterApps();

        // Required to access precompiled Razor views.
        CompiledRazorAssemblyPart part = new(Assembly.GetExecutingAssembly());
        
        services
            .AddMvcCore()
            .AddViews()
            .AddRazorViewEngine()
            .PartManager
            .ApplicationParts
            .Add(part);
        
        services.AddTransient<IRazorRenderer, RazorRenderer>();

        services.AddPaymentsCommon();
        services.AddMembers();
        services.AddReports();
    })
    .Build();

host.Run();
