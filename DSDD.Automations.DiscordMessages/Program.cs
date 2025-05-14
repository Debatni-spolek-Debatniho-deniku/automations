using DSDD.Automations.DiscordMessages;
using Microsoft.Extensions.Hosting;
using DSDD.Automations.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .ConfigureAutomationsFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddOptionsWithValidateOnStart<DiscordMessageOptions>().BindConfiguration("");
    })
    .Build();

host.Run();