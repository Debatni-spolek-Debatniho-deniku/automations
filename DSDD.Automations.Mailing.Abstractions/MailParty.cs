namespace DSDD.Automations.Mailing.Abstractions;

public readonly struct MailParty
{
    public string Name { get; }

    public string Address { get; }

    public MailParty(string name, string address)
    {
        Name = name;
        Address = address;
    }
}