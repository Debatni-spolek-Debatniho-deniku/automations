using DSDD.Automations.Payments.Views.Shared;
using DSDD.Automations.Reports.Members;

namespace DSDD.Automations.Payments.Views;

public class IndexViewModel : LayoutViewModel
{
    public string SisterAppUrl { get; }

    public IReadOnlyCollection<Member> Members { get; }

    public IndexViewModel(string sisterAppUrl, IReadOnlyCollection<Member> members) : base("Výběr poplatníka")
    {
        SisterAppUrl = sisterAppUrl;
        Members = members;
    }
}