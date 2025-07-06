using Newtonsoft.Json;

namespace DSDD.Automations.Payments.Persistence.Abstractions.Model.Payers;

public class Payer
{
    // For CosmosDB
    [JsonProperty("id")]
    private string Id => VariableSymbol.ToString();

    public ulong VariableSymbol { get; }

    public ICollection<PayerManualPayment> ManualPayments { get; } = new List<PayerManualPayment>();

    public ICollection<PayerBankPayment> BankPayments { get; } = new List<PayerBankPayment>();

    public Payer(ulong variableSymbol)
    {
        VariableSymbol = variableSymbol;
    }
}