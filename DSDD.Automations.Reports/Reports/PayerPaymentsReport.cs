using ClosedXML.Excel;
using DSDD.Automations.Payments;
using DSDD.Automations.Payments.Model;
using DSDD.Automations.Reports.Members;

namespace DSDD.Automations.Reports.Reports;

public class PayerPaymentsReport: IPayerPaymentsReport
{
    public PayerPaymentsReport(IMembersProvider membersProvider, IPayersDao payersDao)
    {
        _membersProvider = membersProvider;
        _payersDao = payersDao;
    }

    public async Task<Stream> GenerateXlsxAsync(ulong variableSymbol, ulong? constantSymbol, CancellationToken ct)
    {
        throw new NotImplementedException();

        //IReadOnlyCollection<Member> members = await _membersProvider.GetMembersAsync(ct);

        //Payment[][] payments = await Task.WhenAll(members.Select(async member =>
        //{
        //    Payer? payer = await _payersDao.GetAsync(member.VariableSymbol, ct);
        //    if (payer is null)
        //        return Array.Empty<Payment>();
        //    return payer
        //        .BankPayments
        //        .Select(p => new Payment(p))
        //        .Concat(payer
        //            .ManualPayments
        //            .Select(p => new Payment(p)))
        //        .ToArray();
        //}));
        
        //using XLWorkbook workbook = new();
        //IXLWorksheet worksheet = workbook.AddWorksheet();
        //IXLTable table = worksheet
        //    .FirstCell()
        //    .InsertTable(payments.SelectMany(_ => _).OrderByDescending(p => p.DateTime));
        //table.ShowTotalsRow = true;
        //table.Theme = XLTableTheme.TableStyleMedium18;

        
    }

    private struct Payment
    {
        public string CounterParty { get; }

        public ulong? ConstantSymbol { get; }

        public decimal AmountCzk { get; }

        public DateTime DateTime { get; }

        public string? Description { get; }

        public Payment(BankPayment payment)
        {
            CounterParty = payment.CounterpartyAccountNumber;
            ConstantSymbol = payment.FinalConstantSymbol;
            AmountCzk = payment.AmountCzk;
            DateTime = payment.FinalDateTime;
            Description = payment.FinalDescription;
        }

        public Payment(ManualPayment payment)
        {
            CounterParty = "Manuální";
            ConstantSymbol = payment.ConstantSymbol;
            AmountCzk = payment.AmountCzk;
            DateTime = payment.DateTime;
            Description = payment.Description;
        }
    }

    private readonly IMembersProvider _membersProvider;
    private readonly IPayersDao _payersDao;
}