using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace DSDD.Automations.Reports.Members.Extractor;

public class ClosedXMLMembersExtractorOptions
{
    [ConfigurationKeyName("SPO_MEMBERS_WORKSHEET"), Required]
    public string Worksheet { get; set; } = "";

    [ConfigurationKeyName("SPO_MEMBERS_HEADER_SIZE"), Required]
    public int HeaderSize { get; set; }

    [ConfigurationKeyName("SPO_MEMBERS_FIRST_NAME_COL"), Required]
    public string FirstNameColumn { get; set; } = "";

    [ConfigurationKeyName("SPO_MEMBERS_LAST_NAME_COL"), Required]
    public string LastNameColumn { get; set; } = "";

    [ConfigurationKeyName("SPO_MEMBERS_VARIABLE_SYMBOL_COL"), Required]
    public string VariableSymbolColumn { get; set; } = "";

    [ConfigurationKeyName("SPO_MEMBERS_ENLISTED_COL"), Required]
    public string EnlistedColumn { get; set; } = "";
}