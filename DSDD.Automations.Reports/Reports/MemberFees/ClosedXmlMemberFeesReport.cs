using ClosedXML.Excel;
using DSDD.Automations.Payments;
using DSDD.Automations.Payments.Model;
using DSDD.Automations.Reports.Members;
using Microsoft.Extensions.Options;

namespace DSDD.Automations.Reports.Reports.MemberFees;

public class ClosedXmlMemberFeesReport: IMemberFeesReport
{
    public ClosedXmlMemberFeesReport(IMembersProvider membersProvider, IPayersDao payersDao, IOptions<ClosedXmlMemberFeesReportOptions> options)
    {
        _membersProvider = membersProvider;
        _payersDao = payersDao;
        _options = options.Value;
    }

    public async Task<Stream> GenerateXlsxAsync(CancellationToken ct)
    {
        MonthYear feesFromMonth = new(_options.FromMonth, _options.FromYear);

        IReadOnlyCollection<Member> members = await _membersProvider.GetMembersAsync(ct);
        members = members.OrderBy(p => p.LastName).ToArray();
        
        WriteRowByMonth writeRow = WriteRowByMonthColumnsFactory(
            new(members.Min(m => m.Enlisted)),
            new(DateTime.Today));

        using XLWorkbook workbook = new();
        IXLWorksheet worksheet = workbook.AddWorksheet();

        writeRow(
            worksheet.Row(1),
            cell => cell.Value = XLCellValue.FromObject("Jméno"),
            cell => cell.Value = XLCellValue.FromObject("Přijmení"),
            cell => cell.Value = XLCellValue.FromObject("VS"),
            (cell, monthYear) =>
            {
                ClosedXmlHelpers.ApplyCzkFormat(cell.WorksheetColumn());
                
                cell.Style.DateFormat.Format = "mmmm yyyy";
                cell.Value = XLCellValue.FromObject(new DateTime(monthYear.Year, monthYear.Month, 1));
            });

        int rowNumber = 2;
        foreach (Member member in members)
        {
            MonthYear enlisted = new(member.Enlisted);

            Payer? payer = await _payersDao.GetAsync(member.VariableSymbol, ct);
            payer ??= new(member.VariableSymbol);

            PayerPaymentsPerMonth perMonth = new(payer, _options.ConstantSymbol);
            
            writeRow(
                worksheet.Row(rowNumber),
                cell => cell.Value = XLCellValue.FromObject(member.FirstName),
                cell => cell.Value = XLCellValue.FromObject(member.LastName),
                cell => cell.Value = XLCellValue.FromObject(member.VariableSymbol),
                (cell, monthYear) =>
                {
                    bool thisMonthHasPaymentRequirement = feesFromMonth <= monthYear;
                    if (!thisMonthHasPaymentRequirement)
                        return;

                    bool payerIsEnslitedForThisMonth = enlisted <= monthYear;
                    if (!payerIsEnslitedForThisMonth)
                        return;

                    decimal payedThisMonth = perMonth.Get(monthYear);
                    cell.Value = XLCellValue.FromObject(payedThisMonth);
                    cell.Style.Font.Bold = true;
                    
                    cell.Style.Font.FontColor = _options.FeeCzk <= payedThisMonth ? XLColor.Green : XLColor.Red;
                });

            rowNumber++;
        }

        foreach (IXLColumn column in worksheet.ColumnsUsed())
            column.AdjustToContents();

        IXLRange range = worksheet.RangeUsed();
        range.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
        range
            .Row(1)
            .Style
            .Border
            .SetBottomBorder(XLBorderStyleValues.Medium);
        range
            .Row(1)
            .Style
            .Font
            .Bold = true;
        range
            .Range(2, 3, range.LastRow().RowNumber(), 3)
            .Style
            .Border
            .SetRightBorder(XLBorderStyleValues.Thin);

        return ClosedXmlHelpers.SaveToMemory(workbook);
    }

    private readonly IMembersProvider _membersProvider;
    private readonly IPayersDao _payersDao;
    private readonly ClosedXmlMemberFeesReportOptions _options;
    
    private delegate void WriteRowByMonth(
        IXLRow row,
        Action<IXLCell> setupFirstNameCell,
        Action<IXLCell> setupLastNameCell,
        Action<IXLCell> setupVariableSymbolCell,
        Action<IXLCell, MonthYear> setupCell);

    private WriteRowByMonth WriteRowByMonthColumnsFactory(MonthYear from, MonthYear to)
        => (
            IXLRow row,
            Action<IXLCell> setupFirstNameCell,
            Action<IXLCell> setupLastNameCell,
            Action<IXLCell> setupVariableSymbolCell,
            Action<IXLCell, MonthYear> setupCell) =>
        {
            MonthYear current = from;

            setupFirstNameCell(row.Cell(1));
            setupLastNameCell(row.Cell(2));
            setupVariableSymbolCell(row.Cell(3));

            int cellNumber = 4;
            while (current <= to)
            {
                IXLCell cell = row.Cell(cellNumber);

                setupCell(cell, current);

                DateOnly nextMonth = new DateOnly(current.Year, current.Month, 1).AddMonths(1);
                current = new MonthYear(nextMonth);
                cellNumber++;
            }
        };
}