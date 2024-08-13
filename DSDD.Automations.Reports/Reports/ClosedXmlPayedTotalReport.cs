using System.Data;
using ClosedXML.Excel;
using DSDD.Automations.Payments;
using DSDD.Automations.Reports.Members;

namespace DSDD.Automations.Reports.Reports;

public class ClosedXmlPayedTotalReport: IPayedTotalReport
{
    public ClosedXmlPayedTotalReport(IMembersProvider membersProvider, IPayersDao payersDao)
    {
        _membersProvider = membersProvider;
        _payersDao = payersDao;
    }

    public async Task<Stream> GenerateXlsxAsync(ulong constantSymbol, CancellationToken ct)
    {
        IReadOnlyCollection<Member> members = await _membersProvider.GetMembersAsync(ct);
        Dictionary<ulong, Member> byVariableSymbol = members.ToDictionary(m => m.VariableSymbol);

        SummedPayer[] payers = await _payersDao
            .GetAllAsync(ct)
            .Select(payer =>
            {
                decimal bank = payer.BankPayments.Where(p => p.FinalConstantSymbol == constantSymbol).Sum(p => p.AmountCzk);
                decimal manual = payer.ManualPayments.Where(p => p.ConstantSymbol == constantSymbol).Sum(p => p.AmountCzk);
                decimal total = bank + manual;

                if (!byVariableSymbol.TryGetValue(payer.VariableSymbol, out Member? member))
                    return new SummedPayer(payer.VariableSymbol, total);
                return new SummedPayer(member.FirstName, member.LastName, member.VariableSymbol, total);
            })
            .OrderBy(p => p.LastName)
            .ToArrayAsync(ct);

        return ClosedXmlHelpers.ToSingleTableWorkbook(payers);
    }

    private readonly IMembersProvider _membersProvider;
    private readonly IPayersDao _payersDao;

    private struct SummedPayer
    {
        [ClosedXmlHelpers.Configuration("Jméno")]
        public string FirstName { get; }

        [ClosedXmlHelpers.Configuration("Přjmení")]
        public string LastName { get; }

        [ClosedXmlHelpers.Configuration("VS")]
        public ulong VariableSymbol { get; }

        [ClosedXmlHelpers.Configuration("Částka", IsCzk = true, TotalsFunction = XLTotalsRowFunction.Sum)]
        public decimal AmountCzk { get; }

        public SummedPayer(string firstName, string lastName, ulong variableSymbol, decimal amountCzk)
        {
            FirstName = firstName;
            LastName = lastName;
            VariableSymbol = variableSymbol;
            AmountCzk = amountCzk;
        }

        public SummedPayer(ulong variableSymbol, decimal amountCzk) : this("-", "-", variableSymbol, amountCzk)
        {

        }
    }
}