using DSDD.Automations.Payments.Model;

namespace DSDD.Automations.Payments;

public interface IPayers
{
    Task<Payer?> GetAsync(ulong variableSymbol);

    Task UpsertAync(Payer payer);
}