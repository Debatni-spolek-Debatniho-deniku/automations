namespace DSDD.Automations.Payments.Members;

public interface IMemberNameResolver
{
    Task<string?> GetMemberNameAsync(ulong variableSymbol, CancellationToken ct);
}
