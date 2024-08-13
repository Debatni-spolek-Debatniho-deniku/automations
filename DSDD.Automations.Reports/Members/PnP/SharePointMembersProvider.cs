using DSDD.Automations.Reports.Members.Extractor;
using Microsoft.Extensions.Options;
using PnP.Core.Model.SharePoint;
using PnP.Core.Services;

namespace DSDD.Automations.Reports.Members.PnP;

public class SharePointMembersProvider: IMembersProvider
{
    public SharePointMembersProvider(IPnPContextFactory contextFactory, IMembersExtractor membersExtractor,
        IOptions<SharePointMembersProviderOptions> options)
    {
        _contextFactory = contextFactory;
        _membersExtractor = membersExtractor;
        _options = options.Value;
    }

    public async Task<IReadOnlyCollection<Member>> GetMembersAsync(CancellationToken ct)
    {
       using PnPContext ctx = await _contextFactory.CreateAsync(_options.Site, ct);
        
       IFile membersFile = await ctx.Web.GetFileByIdAsync(_options.MembersDocumentId);

       return _membersExtractor.Extract(await membersFile.GetContentAsync());
    }

    private readonly IPnPContextFactory _contextFactory;
    private readonly IMembersExtractor _membersExtractor;
    private readonly SharePointMembersProviderOptions _options;
}