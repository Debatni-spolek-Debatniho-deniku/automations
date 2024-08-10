namespace DSDD.Automations.Reports.Members;

public interface IMembersExtractor
{
    public IReadOnlyCollection<Member> Extract(Stream content);
}