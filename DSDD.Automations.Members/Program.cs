using DSDD.Automations.Hosting;
using DSDD.Automations.Reports.Members;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureAutomationsFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddMemoryCache();
        services.AddSharePointMembersProvider();
    })
    .Build();

host.Run();
