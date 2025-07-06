using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace DSDD.Automations.Reports.Reports.MemberFees;

public class ClosedXmlMemberFeesReportOptions
{
    [ConfigurationKeyName("MEMBER_FEES_FROM_MONTH"), Required]
    public ushort FromMonth { get; set; }

    [ConfigurationKeyName("MEMBER_FEES_FROM_YEAR"), Required]
    public ushort FromYear { get; set; }
    
    [ConfigurationKeyName("MEMBER_FEES_ANNUALY_CONSTANT_SYMBOL"), Required]
    public ulong AnuallyConstantSymbol { get; set; }

    [ConfigurationKeyName("MEMBER_FEES_MONTHLY_CONSTANT_SYMBOL"), Required]
    public ulong MonthlyConstantSymbol { get; set; }

    [ConfigurationKeyName("MEMBER_FEE_ANNUALY_CZK"), Required]
    public ushort AnnualyFeeCzk { get; set; }

    [ConfigurationKeyName("MEMBER_FEE_MONTHLY_CZK"), Required]
    public ushort MonthlyFeeCzk { get; set; }
}