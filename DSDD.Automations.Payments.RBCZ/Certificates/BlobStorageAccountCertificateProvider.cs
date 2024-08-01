using System.Security.Cryptography.X509Certificates;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;

namespace DSDD.Automations.Payments.RBCZ.Certificates;

public class BlobStorageAccountCertificateProvider: IAccountCertificateProvider
{
    public BlobStorageAccountCertificateProvider(TokenCredential tokenCredential, IOptions<BlobStorageAccountCertificateOptions> options)
    {
        _tokenCredential = tokenCredential;
        _options = options.Value;
    }

    public async Task<X509Certificate> GetAsync(CancellationToken ct)
    {
        BlobServiceClient client = new(new Uri($"https://{_options.AccountNambe}.blob.core.windows.net"), _tokenCredential);

        string[] pathSegments = _options.Path.Split("/");
        if (pathSegments.Length != 2)
            throw new ArgumentException($"Expected path to have two segments but got {pathSegments.Length}!");

        BlobContainerClient containerClient = client.GetBlobContainerClient(pathSegments[0]);
        BlobClient blobClient = containerClient.GetBlobClient(pathSegments[1]);

        BlobDownloadResult result = await blobClient.DownloadContentAsync(ct);

        return new X509Certificate2(result.Content.ToArray(), _options.Password);
    }

    private readonly TokenCredential _tokenCredential;
    private readonly BlobStorageAccountCertificateOptions _options;
}