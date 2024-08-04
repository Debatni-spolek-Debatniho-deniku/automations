namespace DSDD.Automations.Reports.Views.Shared;

public abstract class LayoutViewModel
{
    public string Title { get; }

    protected LayoutViewModel(string title)
    {
        Title = title;
    }
}