﻿using DSDD.Automations.Payments.Model;

namespace DSDD.Automations.Payments;

public interface IPayersDao
{
    Task<Payer?> GetAsync(ulong variableSymbol, CancellationToken ct);

    IAsyncEnumerable<Payer> GetAllAsync(CancellationToken ct);

    Task UpsertAync(Payer payer, CancellationToken ct);
}