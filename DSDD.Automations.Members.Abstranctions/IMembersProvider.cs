namespace DSDD.Automations.Reports.Members;

public interface IMembersProvider
{
    Task<IReadOnlyCollection<Member>> GetMembersAsync(CancellationToken ct);
}