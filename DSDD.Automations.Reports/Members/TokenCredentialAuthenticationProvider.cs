using System.Net.Http.Headers;
using Azure.Core;
using PnP.Core.Services;

namespace DSDD.Automations.Reports.Members;

public class TokenCredentialAuthenticationProvider : IAuthenticationProvider
{
    public TokenCredentialAuthenticationProvider(TokenCredential tokenCredential)
    {
        _tokenCredential = tokenCredential;
    }

    public async Task AuthenticateRequestAsync(Uri resource, HttpRequestMessage request)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "bearer",
            await GetAccessTokenAsync(resource));
    }

    public async Task<string> GetAccessTokenAsync(Uri resource, string[] scopes)
    {
        AccessToken token = await _tokenCredential.GetTokenAsync(new(scopes), CancellationToken.None);
        return token.Token;
    }

    public Task<string> GetAccessTokenAsync(Uri resource)
        => GetAccessTokenAsync(resource, GetRelevantScopes(resource));

    private readonly TokenCredential _tokenCredential;
    
    private string[] GetRelevantScopes(Uri resourceUri)
        => [$"{resourceUri.Scheme}://{resourceUri.DnsSafeHost}//.default"];
}