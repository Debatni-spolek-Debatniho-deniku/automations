using Newtonsoft.Json;

namespace DSDD.Automations.Payments.Model;

public class Payer
{
    // For CosmosDB
    [JsonProperty("id")]
    private string Id => VariableSymbol.ToString();

    public ulong VariableSymbol { get; }

    public ICollection<ManualPayment> ManualPayments { get; } = new List<ManualPayment>();

    public ICollection<BankPayment> BankPayments { get; } = new List<BankPayment>();

    public Payer(ulong variableSymbol)
    {
        VariableSymbol = variableSymbol;
    }
}