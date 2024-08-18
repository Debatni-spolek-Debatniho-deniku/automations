using System.Reflection;
using DSDD.Automations.Hosting;
using DSDD.Automations.Hosting.Razor;
using DSDD.Automations.Hosting.SisterApps;
using DSDD.Automations.Payments;
using DSDD.Automations.Reports.Members;
using DSDD.Automations.Reports.Middleware;
using Microsoft.Extensions.Hosting;
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
        
        services.AddRazorRenderer(new CompiledRazorAssemblyPart(Assembly.GetExecutingAssembly()));

        services.AddPaymentsCommon();
        services.AddMembers();
        services.AddReports();
    })
    .Build();

host.Run();
