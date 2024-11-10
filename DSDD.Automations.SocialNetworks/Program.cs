using DSDD.Automations.Hosting;
using DSDD.Automations.Mailing;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureAutomationsFunctionsWebApplication(app =>
    {
        app.Services.AddMailing();
    })
    .Build();

host.Run();