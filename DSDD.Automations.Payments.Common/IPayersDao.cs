using DSDD.Automations.Payments.Model;

namespace DSDD.Automations.Payments;

public interface IPayersDao
{
    Task<Payer?> GetAsync(ulong variableSymbol);

    Task UpsertAync(Payer payer);
}