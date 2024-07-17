using DSDD.Automations.Payments.Views.Layout;

namespace DSDD.Automations.Payments.Views.Error;

public class ErrorViewModel: LayoutViewModel
{
    public Exception Exception { get; }

    public ErrorViewModel(Exception exception) : base("Error")
    {
        Exception = exception;
    }
}