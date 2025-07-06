using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace DSDD.Automations.Payments.Persistence.Cosmos;

public class CosmosOptions
{
    [ConfigurationKeyName("COSMOS_DB_ACCOUNT_ENDPOINT"), Required]
    public string AccountEndpoint { get; set; } = "";
}