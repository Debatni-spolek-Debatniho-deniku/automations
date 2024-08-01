using System.Security.Cryptography.X509Certificates;

namespace DSDD.Automations.Payments.RBCZ.Certificates;

public interface IAccountCertificateProvider
{
    Task<X509Certificate> GetAsync(CancellationToken ct);
}