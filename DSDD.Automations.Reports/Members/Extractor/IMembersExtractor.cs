namespace DSDD.Automations.Reports.Members.Extractor;

public interface IMembersExtractor
{
    public IReadOnlyCollection<Member> Extract(Stream content);
}