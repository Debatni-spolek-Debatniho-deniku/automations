using DSDD.Automations.Payments.Persistence.Abstractions.Model.Payers;

namespace DSDD.Automations.Payments.Persistence.Abstractions;

public interface IPayersDao
{
    Task<Payer?> GetAsync(ulong variableSymbol, CancellationToken ct);

    IAsyncEnumerable<Payer> GetAllAsync(CancellationToken ct);

    Task UpsertAync(Payer payer, CancellationToken ct);
}