using Microsoft.Extensions.Options;
using PnP.Core.Model.SharePoint;
using PnP.Core.Services;

namespace DSDD.Automations.Reports.Members;

public class SharePointMembersProvider: IMembersProvider
{
    public SharePointMembersProvider(IPnPContextFactory contextFactory, IOptions<SharePointMembersProviderOptions> options)
    {
        _contextFactory = contextFactory;
        _options = options.Value;
    }

    public async Task<IReadOnlyCollection<Member>> GetMembersAsync(CancellationToken ct)
    {
       using PnPContext ctx = await _contextFactory.CreateAsync(_options.Site, ct);
        
       IFile membersFile = await ctx.Web.GetFileByIdAsync(_options.MembersDocumentId);

       return null!;
    }

    private readonly IPnPContextFactory _contextFactory;

    private readonly SharePointMembersProviderOptions _options;
}