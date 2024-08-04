namespace DSDD.Automations.Reports.Members;

public class Member
{
    public string FirstName { get; }

    public string LastName { get; }

    public int VariableSymbol { get; }

    public Member(string firstName, string lastName, int variableSymbol)
    {
        FirstName = firstName;
        LastName = lastName;
        VariableSymbol = variableSymbol;
    }
}