using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace DSDD.Automations.Hosting.SisterApps;

public class SisterAppsOptions
{
    [ConfigurationKeyName("SISTER_APP_URL"), Required]
    public string SisterAppUrl { get; set; } = "";
}