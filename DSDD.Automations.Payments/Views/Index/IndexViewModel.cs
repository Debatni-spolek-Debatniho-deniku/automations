using DSDD.Automations.Payments.Views.Layout;

namespace DSDD.Automations.Payments.Views.Index;

public class IndexViewModel : LayoutViewModel
{
    public string SisterAppUrl { get; }

    public IndexViewModel(string sisterAppUrl) : base("Výběr poplatníka")
    {
        SisterAppUrl = sisterAppUrl;
    }
}