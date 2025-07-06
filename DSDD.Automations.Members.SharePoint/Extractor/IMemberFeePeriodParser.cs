namespace DSDD.Automations.Reports.Members.Extractor;

public interface IMemberFeePeriodParser
{
    MemberFeePaymentPeriod Parse(string value);
}