namespace DSDD.Automations.Payments.ViewModel;

public abstract class LayoutViewModel
{
    public string Title { get; }

    public LayoutViewModel(string title)
    {
        Title = title;
    }
}