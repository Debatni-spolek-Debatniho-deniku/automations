namespace DSDD.Automations.Payments.Views;

public class CalloutBackButtonColor
{
    public static CalloutBackButtonColor DANGER = new("danger");

    public static CalloutBackButtonColor SUCCESS = new("success");
    
    #region Instance

    private readonly string value;

    private CalloutBackButtonColor(string value)
    {
        this.value = value;
    }

    public override string ToString()
        => value;

    #endregion
}