namespace DSDD.Automations.Payments.Views.Layout;

public abstract class LayoutViewModel
{
    public string Title { get; }

    protected LayoutViewModel(string title)
    {
        Title = title;
    }
}