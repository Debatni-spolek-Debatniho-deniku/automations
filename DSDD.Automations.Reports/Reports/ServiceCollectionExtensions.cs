using Microsoft.Extensions.DependencyInjection;

namespace DSDD.Automations.Reports.Reports;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReports(this IServiceCollection services)
    {
        services.AddTransient<IMemberFeesReport, ClosedXmlMemberFeesReport>();
        services.AddTransient<IPayerPaymentsReport, ClosedXmlPayerPaymentsReport>();
        services.AddTransient<IPayedTotalReport, ClosedXmlPayedTotalReport>();

        return services;
    }
}