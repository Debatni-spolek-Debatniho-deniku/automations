namespace DSDD.Automations.Mailing.Abstractions;

public struct MailAttachment
{
    public string Name { get; }

    public string ContentType { get; }

    public Stream Stream { get; }

    public MailAttachment(string name, string contentType, Stream stream)
    {
        Name = name;
        ContentType = contentType;
        Stream = stream;
    }
}