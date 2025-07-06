namespace DSDD.Automations.Payments.Helpers.Tests;

public class NumericSymbolParserTests
{
    [TestCase("4", 4UL)]
    [TestCase("999999999", 999999999UL)]
    [TestCase("00000000080", 80UL)]
    public void Parse(string raw, ulong expected)
    {
        ulong result = _sut.Parse(raw);
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_Throws()
        => Assert.Throws<NumericSymbolParser.NumericSymbolCannotBeParsed>(() => _sut.Parse("SSS400"));

    private readonly NumericSymbolParser _sut = new();
}