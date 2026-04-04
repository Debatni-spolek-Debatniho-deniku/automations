using DSDD.Automations.Reports.Members;

namespace DSDD.Automations.Payments.Members;

public class MemberNameResolver : IMemberNameResolver
{
    private readonly IMembersProvider _membersProvider;

    public MemberNameResolver(IMembersProvider membersProvider)
    {
        _membersProvider = membersProvider;
    }

    public async Task<string?> GetMemberNameAsync(ulong variableSymbol, CancellationToken ct)
    {
        IReadOnlyCollection<Member> members = await _membersProvider.GetMembersAsync(ct);
        Member? member = members.FirstOrDefault(m => m.VariableSymbol == variableSymbol);
        return member is not null ? $"{member.FirstName} {member.LastName}" : null;
    }
}
