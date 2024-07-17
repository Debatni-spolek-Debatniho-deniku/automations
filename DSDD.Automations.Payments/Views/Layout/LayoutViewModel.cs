namespace DSDD.Automations.Payments.Views.Layout;

public abstract class LayoutViewModel
{
    public string Title { get; }

    public Exception? Exception { get; }

    protected LayoutViewModel(string title, Exception? exception)
    {
        Title = title;
        Exception = exception;
    }
}