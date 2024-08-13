using DSDD.Automations.Reports.Views.Shared;

namespace DSDD.Automations.Reports.Views;

public class IndexViewModel : LayoutViewModel
{
    public string SisterAppUrl { get; } 

    public IndexViewModel(string sisterAppUrl) : base("Reporty")
    {
        SisterAppUrl = sisterAppUrl;
    }
}