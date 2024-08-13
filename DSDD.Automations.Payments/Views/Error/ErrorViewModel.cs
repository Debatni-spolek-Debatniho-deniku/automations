using DSDD.Automations.Payments.Views.Layout;

namespace DSDD.Automations.Payments.Views.Error;

public class ErrorViewModel: LayoutViewModel
{
    public string Message { get; }

    public ErrorViewModel(Exception exception) : this(exception.Message)
    {

    }

    public ErrorViewModel(string message) : base("Error")
    {
        Message = message;
    }
}