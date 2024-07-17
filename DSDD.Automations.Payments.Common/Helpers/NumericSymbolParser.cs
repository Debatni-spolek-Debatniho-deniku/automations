namespace DSDD.Automations.Payments.Helpers;

public class NumericSymbolParser : INumericSymbolParser
{
    public class NumericSymbolCannotBeParsed : ArgumentException
    {
        public NumericSymbolCannotBeParsed(string variableSymbol) : base($"Číselný symbol {variableSymbol} není validní!")
        { }
    }

    public ulong Parse(string variableSymbol)
        => ulong.TryParse(variableSymbol, out ulong result)
            ? result
            : throw new NumericSymbolCannotBeParsed(variableSymbol);
}