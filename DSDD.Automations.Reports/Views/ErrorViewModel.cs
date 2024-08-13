using DSDD.Automations.Reports.Views.Shared;

namespace DSDD.Automations.Reports.Views;

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