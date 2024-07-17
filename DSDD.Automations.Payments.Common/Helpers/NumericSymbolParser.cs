namespace DSDD.Automations.Payments.Helpers;

public class NumericSymbolParser : INumericSymbolParser
{
    public class VariableSymbolCannotBeParsed : ArgumentException
    {
        public VariableSymbolCannotBeParsed(string variableSymbol) : base($"Varaible symbol {variableSymbol} cannot be parsed!")
        { }
    }

    public ulong Parse(string variableSymbol)
        => ulong.TryParse(variableSymbol, out ulong result)
            ? result
            : throw new VariableSymbolCannotBeParsed(variableSymbol);
}