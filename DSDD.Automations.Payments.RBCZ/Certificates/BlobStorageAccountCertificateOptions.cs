using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace DSDD.Automations.Payments.RBCZ.Certificates;

public class BlobStorageAccountCertificateOptions
{
    [ConfigurationKeyName("RBCZ_BLOB_STORAGE_ACCOUNT"), Required]
    public string AccountNambe { get; set; } = "";

    [ConfigurationKeyName("RBCZ_ACCOUNT_CERTIFICATE_PATH"), Required, RegularExpression("^[^\\/]+\\/[^\\/]+$")]
    public string Path { get; set; } = "";

    [ConfigurationKeyName("RBCZ_ACCOUNT_CERTIFICATE_PASSWORD"), Required]
    public string Password { get; set; } = "";
}