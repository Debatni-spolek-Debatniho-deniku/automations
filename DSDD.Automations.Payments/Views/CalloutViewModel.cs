using DSDD.Automations.Payments.Views.Shared;

namespace DSDD.Automations.Payments.Views;

public class CalloutViewModel : LayoutViewModel
{
    public string Message { get; }

    public CalloutBackButtonColor BackButtonColor { get; }

    public CalloutViewModel(string title, string message, CalloutBackButtonColor backButtonColor) : base(title)
    {
        Message = message;
        BackButtonColor = backButtonColor;
    }

    public static CalloutViewModel Error(string message)
        => new("Error", message, CalloutBackButtonColor.DANGER);
}