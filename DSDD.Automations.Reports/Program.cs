using System.Reflection;
using DSDD.Automations.Hosting;
using DSDD.Automations.Hosting.Razor;
using DSDD.Automations.Hosting.SisterApps;
using DSDD.Automations.Mailing;
using DSDD.Automations.Payments;
using DSDD.Automations.Reports;
using DSDD.Automations.Reports.Members;
using DSDD.Automations.Reports.Middleware;
using Microsoft.Extensions.Hosting;
using DSDD.Automations.Reports.Reports;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddMailing();
        
        services.AddOptionsWithValidateOnStart<NotifyMemberFeesOptions>().BindConfiguration("");
    })
    .Build();

host.Run();
