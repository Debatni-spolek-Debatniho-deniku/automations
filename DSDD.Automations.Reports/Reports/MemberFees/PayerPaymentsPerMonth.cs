using DSDD.Automations.Payments.Model;

namespace DSDD.Automations.Reports.Reports.MemberFees;

public struct PayerPaymentsPerMonth
{
    public PayerPaymentsPerMonth(Payer payer, ulong constantSymbol)
    {
        _perMonth = payer
            .ManualPayments
            .Select(p => (p.DateTime, p.ConstantSymbol, p.AmountCzk))
            .Concat(payer
                .BankPayments
                .Where(p => !p.Overrides.Removed)
                .Select(p => (p.DateTime, p.ConstantSymbol, p.AmountCzk)))
            .Where(p => p.ConstantSymbol == constantSymbol)
            .Select(p => (new MonthYear(p.DateTime), p.AmountCzk))
            .GroupBy(p => p.Item1, p => p.AmountCzk)
            .ToDictionary(p => p.Key, p => p.Sum());
    }

    /// <summary>
    /// Returs total amount payer payed in given month or 0 if paid none.
    /// </summary>
    /// <param name="monthYear"></param>
    /// <returns></returns>
    public decimal Get(MonthYear monthYear)
        => _perMonth.TryGetValue(monthYear, out decimal d) ? d : 0;

    private readonly IReadOnlyDictionary<MonthYear, decimal> _perMonth;
}