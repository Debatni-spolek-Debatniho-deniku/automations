using DSDD.Automations.Hosting;
using DSDD.Automations.Reports.Middleware;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .ConfigureAutomationsFunctionsWebApplication(app =>
    {
        app.UseMiddleware<ReportsReaderAuthorizationMiddleware>();
    })
    .ConfigureServices(services =>
    {
    })
    .Build();

host.Run();
