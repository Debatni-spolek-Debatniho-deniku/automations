namespace DSDD.Automations.Reports.Members;

public class Member
{
    public string FirstName { get; }

    public string LastName { get; }

    public ulong VariableSymbol { get; }

    public DateOnly Enlisted { get; }

    public Member(string firstName, string lastName, ulong variableSymbol, DateOnly enlisted)
    {
        FirstName = firstName;
        LastName = lastName;
        VariableSymbol = variableSymbol;
    }
}