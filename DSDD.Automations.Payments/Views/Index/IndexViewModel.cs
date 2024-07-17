using DSDD.Automations.Payments.Views.Layout;

namespace DSDD.Automations.Payments.Views.Index;

public class IndexViewModel : LayoutViewModel
{
    public IndexViewModel() : base("Výběr plátce", null)
    {
    }

    public IndexViewModel(Exception exception) : base("Výběr poplatníka", exception)
    {
    }
}