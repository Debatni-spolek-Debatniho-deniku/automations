using DSDD.Automations.Payments.Views.Shared;

namespace DSDD.Automations.Payments.Views;

public class IndexViewModel : LayoutViewModel
{
    public string SisterAppUrl { get; }

    public IndexViewModel(string sisterAppUrl) : base("Výběr poplatníka")
    {
        SisterAppUrl = sisterAppUrl;
    }
}