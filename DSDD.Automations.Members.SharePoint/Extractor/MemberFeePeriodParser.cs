using Microsoft.Extensions.Options;

namespace DSDD.Automations.Reports.Members.Extractor;

public class MemberFeePeriodParser: IMemberFeePeriodParser
{
    public MemberFeePeriodParser(IOptions<ClosedXMLMembersExtractorOptions> options)
    {
        this.options = options;
    }

    public MemberFeePaymentPeriod Parse(string value)
    {
        if (value.Equals(options.Value.FeePaymentPeriodMonthlyValue, StringComparison.InvariantCultureIgnoreCase))
            return MemberFeePaymentPeriod.MONTHLY;
        if (value.Equals(options.Value.FeePaymentPeriodAnnualyValue, StringComparison.InvariantCultureIgnoreCase))
            return MemberFeePaymentPeriod.ANNUALLY;

        throw new ArgumentException($"Not known value {value}");
    }

    private readonly IOptions<ClosedXMLMembersExtractorOptions> options;
}