using ClosedXML.Excel;
using DSDD.Automations.Payments.Helpers;
using Microsoft.Extensions.Options;

namespace DSDD.Automations.Reports.Members.Extractor;

public class ClosedXMLMembersExtractor: IMembersExtractor
{
    public ClosedXMLMembersExtractor(IOptions<ClosedXMLMembersExtractorOptions> options, INumericSymbolParser numericSymbolParser)
    {
        _options = options.Value;
        _numericSymbolParser = numericSymbolParser;
    }

    public IReadOnlyCollection<Member> Extract(Stream content)
    {
        XLWorkbook workbook = new XLWorkbook(content);
        IXLWorksheet membersSheet = workbook.Worksheet(_options.Worksheet);

        List<Member> members = new();
        foreach (IXLRow row in membersSheet.Rows().Skip(_options.HeaderSize))
        {
            string? firstName = row
                .Cell(_options.FirstNameColumn)
                .Value
                .ToString();

            string? lastName = row
                .Cell(_options.LastNameColumn)
                .Value
                .ToString();

            // As excel might contain "helper rows" we decide when the member list ends by absence of name.
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                break;

            ulong variableSymbol = _numericSymbolParser
                .Parse(row
                    .Cell(_options.VariableSymbolColumn)
                    .Value
                    .ToString());

            DateOnly enlisted = DateOnly.FromDateTime(row
                .Cell(_options.EnlistedColumn)
                .Value
                .GetDateTime());

            members.Add(new(firstName, lastName, variableSymbol, enlisted));
        }
        return members;
    }

    private readonly ClosedXMLMembersExtractorOptions _options;
    private readonly INumericSymbolParser _numericSymbolParser;
}