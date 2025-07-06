using DSDD.Automations.Payments.Persistence.Abstractions.Model.Payers;

namespace DSDD.Automations.Payments.Persistence.Abstractions;

public static class PayersDaoExtensions
{
    public static async Task<Payer> GetRequiredAsync(this IPayersDao payers, ulong variableSymbol, CancellationToken ct)
    {
        Payer? maybePayer = await payers.GetAsync(variableSymbol, ct);
        if (maybePayer is null)
            throw new NullReferenceException($"Poplatník s variabilním symbolem {variableSymbol} nebyl nalezen!");
        return maybePayer;
    }
}