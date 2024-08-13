using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace DSDD.Automations.Reports.Reports.MemberFees;

public class ClosedXmlMemberFeesReportOptions
{
    [ConfigurationKeyName("MEMBER_FEES_FROM_MONTH"), Required]
    public ushort FromMonth { get; set; }

    [ConfigurationKeyName("MEMBER_FEES_FROM_YEAR"), Required]
    public ushort FromYear { get; set; }
    
    [ConfigurationKeyName("MEMBER_FEES_CONSTANT_SYMBOL"), Required]
    public ulong ConstantSymbol { get; set; }

    [ConfigurationKeyName("MEMBER_FEE_CZK"), Required]
    public ushort FeeCzk { get; set; }
}