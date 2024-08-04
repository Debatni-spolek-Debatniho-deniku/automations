using System.Reflection;
using DSDD.Automations.Hosting;
using DSDD.Automations.Reports.Middleware;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using DSDD.Automations.Reports.Razor;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

var host = new HostBuilder()
    .ConfigureAutomationsFunctionsWebApplication(app =>
    {
        app.UseMiddleware<ReportsReaderAuthorizationMiddleware>();
    })
    .ConfigureServices(services =>
    {
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
    })
    .Build();

host.Run();
