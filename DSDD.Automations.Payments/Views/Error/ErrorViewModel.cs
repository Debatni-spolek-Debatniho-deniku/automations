using DSDD.Automations.Payments.Views.Layout;
using Microsoft.AspNetCore.Components;

namespace DSDD.Automations.Payments.Views.Error;

public class ErrorViewModel: LayoutViewModel
{
    public new Exception Exception { get; }

    public ErrorViewModel(Exception exception) : base("Error", null)
    {
        Exception = exception;
    }
}