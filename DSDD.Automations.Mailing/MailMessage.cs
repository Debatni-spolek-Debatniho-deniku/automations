namespace DSDD.Automations.Mailing;

public readonly struct MailMessage
{
    public IReadOnlyList<MailParty> Recipients { get; }

    public string Subject { get; }

    public string Body { get; }

    public MailMessage(IEnumerable<MailParty> recipients, string subject, string body)
    {
        Recipients = recipients.ToArray();
        Subject = subject;
        Body = body;
    }
}