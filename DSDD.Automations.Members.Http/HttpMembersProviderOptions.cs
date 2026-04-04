using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace DSDD.Automations.Members.Http;

public class HttpMembersProviderOptions
{
    [ConfigurationKeyName("MEMBERS_APP_URL"), Required]
    public Uri BaseUrl { get; set; } = null!;

    [ConfigurationKeyName("MEMBERS_APP_KEY"), Required]
    public string FunctionKey { get; set; } = "";
}
