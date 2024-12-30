using DSDD.Automations.Reports.Reports.MemberFees;
using DSDD.Automations.Reports.Reports.PayedTotal;
using DSDD.Automations.Reports.Reports.PayerPayments;
using Microsoft.Extensions.DependencyInjection;

namespace DSDD.Automations.Reports.Reports;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReports(this IServiceCollection services)
    {
        services.AddOptionsWithValidateOnStart<ClosedXmlMemberFeesReportOptions>().BindConfiguration("");

        services.AddTransient<IMemberFeesReport, ClosedXmlMemberFeesReport>();
        services.AddTransient<IPayerPaymentsReport, ClosedXmlPayerPaymentsReport>();
        services.AddTransient<IPayedTotalReport, ClosedXmlPayedTotalReport>();

        return services;
    }
}