namespace DSDD.Automations.Mailing;

public readonly struct MailMessage
{
    public IReadOnlyList<MailParty> Recipients { get; }

    public string Subject { get; }

    public string HtmlBody { get; }

    public IReadOnlyList<MailAttachment> Attachments { get; }

    public MailMessage(IEnumerable<MailParty> recipients, string subject, string htmlBody, params MailAttachment[] attachments)
    {
        Recipients = recipients.ToArray();
        Subject = subject;
        HtmlBody = htmlBody;
        Attachments = attachments;
    }

    public MailMessage(MailParty recipient, string subject, string htmlBody, params MailAttachment[] attachments)
        : this([recipient], subject, htmlBody, attachments)
    {

    }
}