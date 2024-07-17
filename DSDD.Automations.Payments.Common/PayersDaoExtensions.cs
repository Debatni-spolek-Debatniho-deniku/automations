using DSDD.Automations.Payments.Model;

namespace DSDD.Automations.Payments;

public static class PayersDaoExtensions
{
    public static async Task<Payer> GetRequiredAsync(this IPayersDao payers, ulong variableSymbol)
    {
        Payer? maybePayer = await payers.GetAsync(variableSymbol);
        if (maybePayer is null)
            throw new NullReferenceException($"Poplatník s variabilním symbolem {variableSymbol} nebyl nalezen!");
        return maybePayer;
    }
}