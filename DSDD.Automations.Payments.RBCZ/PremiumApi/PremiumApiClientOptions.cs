using Microsoft.Extensions.Configuration;

namespace DSDD.Automations.Payments.RBCZ.PremiumApi;

public class PremiumApiClientOptions
{
    [ConfigurationKeyName("RBCZ_CLIENT_ID")]
    public string ClientId { get; set; } = "";

    [ConfigurationKeyName("RBCZ_AUDIT_IP_ADDRESS")]
    public string AuditIpAddress { get; set; } = "";

    [ConfigurationKeyName("RBCZ_ACCOUNT_NUMBER")]
    public string AccountNumber { get; set; } = "";

    [ConfigurationKeyName("RBCZ_ACCOUNT_CURRENCY")]
    public string AccountCurrency { get; set; } = "";

    [ConfigurationKeyName("RBCZ_CERTIFICATE_PATH")]
    public string CertificatePath { get; set; } = "";

    [ConfigurationKeyName("RBCZ_CERTIFICATE_PASSWORD")]
    public string CertificatePassword { get; set; } = "";

    [ConfigurationKeyName("RBCZ_USE_SANDBOX_API")]
    public bool UseSandboxApi { get; set; }
}