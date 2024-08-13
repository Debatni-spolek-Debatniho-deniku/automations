using System.Data;
using ClosedXML.Excel;
using DSDD.Automations.Payments;
using DSDD.Automations.Payments.Model;
using DSDD.Automations.Reports.Members;

namespace DSDD.Automations.Reports.Reports;

public class PayedTotalReport: IPayedTotalReport
{
    public PayedTotalReport(IMembersProvider membersProvider, IPayersDao payersDao)
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
            .ToArrayAsync(ct);

        using XLWorkbook workbook = new();
        IXLWorksheet worksheet = workbook.AddWorksheet();
        IXLTable table = worksheet.FirstCell().InsertTable(payers.OrderBy(p => p.LastName));
        table.ShowTotalsRow = true;
        table.Theme = XLTableTheme.TableStyleMedium18;

        IXLTableField amountCzk = table.Field(nameof(SummedPayer.AmountCzk));
        amountCzk.Name = "Částka";
        amountCzk.TotalsRowFunction = XLTotalsRowFunction.Sum;
        amountCzk.Column.Style.NumberFormat.Format = "# ##0.00 \"CZK\"";

        IXLTableField firstName = table.Field(nameof(SummedPayer.FirstName));
        firstName.Name = "Jméno";

        IXLTableField lastName = table.Field(nameof(SummedPayer.LastName));
        lastName.Name = "Přijmení";

        IXLTableField varaibleSymbol = table.Field(nameof(SummedPayer.VariableSymbol));
        varaibleSymbol.Name = "VS";

        foreach (IXLRangeColumn column in table.ColumnsUsed())
            column.WorksheetColumn().AdjustToContents();

        MemoryStream result = new();
        workbook.SaveAs(result);
        result.Seek(0, SeekOrigin.Begin);

        return result;
    }

    private readonly IMembersProvider _membersProvider;
    private readonly IPayersDao _payersDao;

    private struct SummedPayer
    {
        public string FirstName { get; }

        public string LastName { get; }

        public ulong VariableSymbol { get; }

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