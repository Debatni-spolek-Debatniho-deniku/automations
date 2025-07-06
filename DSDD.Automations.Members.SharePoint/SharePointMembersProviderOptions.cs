using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace DSDD.Automations.Members.SharePoint;

public class SharePointMembersProviderOptions
{
    [ConfigurationKeyName("SPO_SITE_URL"), Required]
    public Uri Site { get; set; } = null!;

    [ConfigurationKeyName("SPO_MEMBERS_DOCUMENT_ID"), Required]
    public Guid MembersDocumentId { get; set; } = Guid.Empty;
}