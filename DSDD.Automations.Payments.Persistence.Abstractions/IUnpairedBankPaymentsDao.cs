using DSDD.Automations.Payments.Persistence.Abstractions.Model;

namespace DSDD.Automations.Payments.Persistence.Abstractions;

public interface IUnpairedBankPaymentsDao
{
    Task<UnpairedBankPayment?> GetAsync(string paymentReference, CancellationToken ct);

    IAsyncEnumerable<UnpairedBankPayment> GetAllAsync(CancellationToken ct);

    Task UpsertAync(UnpairedBankPayment payer, CancellationToken ct);
}