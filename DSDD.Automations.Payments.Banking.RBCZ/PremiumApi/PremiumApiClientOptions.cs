using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace DSDD.Automations.Payments.Banking.RBCZ.PremiumApi;

public class PremiumApiClientOptions
{
    [ConfigurationKeyName("RBCZ_CLIENT_ID"), Required]
    public string ClientId { get; set; } = "";

    [ConfigurationKeyName("RBCZ_AUDIT_IP_ADDRESS"), Required]
    public string AuditIpAddress { get; set; } = "";

    [ConfigurationKeyName("RBCZ_ACCOUNT_NUMBER"), Required]
    public string AccountNumber { get; set; } = "";

    [ConfigurationKeyName("RBCZ_ACCOUNT_CURRENCY"), Required]
    public string AccountCurrency { get; set; } = "";

    [ConfigurationKeyName("RBCZ_CERTIFICATE_PATH"), Required]
    public string CertificatePath { get; set; } = "";

    [ConfigurationKeyName("RBCZ_CERTIFICATE_PASSWORD"), Required]
    public string CertificatePassword { get; set; } = "";

    [ConfigurationKeyName("RBCZ_USE_SANDBOX_API")]
    public bool UseSandboxApi { get; set; } = false;
}