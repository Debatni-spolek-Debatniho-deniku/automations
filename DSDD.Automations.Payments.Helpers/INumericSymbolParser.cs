namespace DSDD.Automations.Payments.Helpers;

public interface INumericSymbolParser
{
    ulong Parse(string variableSymbol);
}