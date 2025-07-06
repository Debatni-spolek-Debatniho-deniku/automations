using DSDD.Automations.Payments.Persistence.Abstractions;
using DSDD.Automations.Payments.Persistence.Abstractions.Model.Payers;

namespace DSDD.Automations.Reports.Reports.PayerPayments;

public class ClosedXmlPayerPaymentsReport: IPayerPaymentsReport
{
    public ClosedXmlPayerPaymentsReport(IPayersDao payersDao)
    {
        _payersDao = payersDao;
    }

    public async Task<ReportFile> GenerateXlsxAsync(ulong variableSymbol, ulong? constantSymbol, CancellationToken ct)
    {
        Payer? payer = await _payersDao.GetAsync(variableSymbol, ct);
        payer ??= new(variableSymbol);

        Payment[] payments = payer
            .BankPayments
            .Select(p => new Payment(p))
            .Concat(payer
                .ManualPayments
                .Select(p => new Payment(p)))
            .Where(p =>
            {
                if (constantSymbol is not { } cs)
                    return true;
                return p.ConstantSymbol == cs;
            })
            .ToArray();

        Stream stream = ClosedXmlHelpers.ToSingleTableWorkbook(payments);
        
        return ReportFile.FromXlsx($"payer-{variableSymbol}-payments{(constantSymbol is ulong cs ? $"-on-{cs}" : "")}", stream);
    }

    private struct Payment
    {
        [ClosedXmlHelpers.Configuration("Protiúčet")]
        public string CounterParty { get; }

        [ClosedXmlHelpers.Configuration("KS")]
        public ulong? ConstantSymbol { get; }

        [ClosedXmlHelpers.Configuration("Částka", IsCzk = true)]
        public decimal AmountCzk { get; }

        [ClosedXmlHelpers.Configuration("Datum a čas")]
        public DateTime DateTime { get; }

        [ClosedXmlHelpers.Configuration("Popis")]
        public string? Description { get; }

        public Payment(PayerBankPayment payment)
        {
            CounterParty = payment.CounterpartyAccountNumber;
            ConstantSymbol = payment.FinalConstantSymbol;
            AmountCzk = payment.AmountCzk;
            DateTime = payment.FinalDateTime;
            Description = payment.FinalDescription;
        }

        public Payment(PayerManualPayment payment)
        {
            CounterParty = "Manuální";
            ConstantSymbol = payment.ConstantSymbol;
            AmountCzk = payment.AmountCzk;
            DateTime = payment.DateTime;
            Description = payment.Description;
        }
    }
    
    private readonly IPayersDao _payersDao;
}