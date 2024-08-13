using ClosedXML.Excel;
using DSDD.Automations.Payments;
using DSDD.Automations.Payments.Model;

namespace DSDD.Automations.Reports.Reports;

public class ClosedXmlPayerPaymentsReport: IPayerPaymentsReport
{
    public ClosedXmlPayerPaymentsReport(IPayersDao payersDao)
    {
        _payersDao = payersDao;
    }

    public async Task<Stream> GenerateXlsxAsync(ulong variableSymbol, ulong? constantSymbol, CancellationToken ct)
    {
        Payment[] payments = await _payersDao
            .GetAllAsync(ct)
            .SelectMany(payer => payer
                .BankPayments
                .Select(p => new Payment(p))
                .Concat(payer
                    .ManualPayments
                    .Select(p => new Payment(p)))
                .ToAsyncEnumerable()
            )
            .Where(p =>
            {
                if (constantSymbol is not { } cs)
                    return true;
                return p.ConstantSymbol == cs;
            })
            .OrderByDescending(p => p.DateTime)
            .ToArrayAsync(ct);


        return ClosedXmlHelpers.ToSingleTableWorkbook(payments);
    }

    private struct Payment
    {
        [ClosedXmlHelpers.Configuration("Protúčet")]
        public string CounterParty { get; }

        [ClosedXmlHelpers.Configuration("KS")]
        public ulong? ConstantSymbol { get; }

        [ClosedXmlHelpers.Configuration("Částka", IsCzk = true)]
        public decimal AmountCzk { get; }

        [ClosedXmlHelpers.Configuration("Datum a čas")]
        public DateTime DateTime { get; }

        [ClosedXmlHelpers.Configuration("Popis")]
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
    
    private readonly IPayersDao _payersDao;
}