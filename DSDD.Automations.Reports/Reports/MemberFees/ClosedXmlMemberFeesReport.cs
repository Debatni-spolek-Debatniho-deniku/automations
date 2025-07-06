using ClosedXML.Excel;
using DSDD.Automations.Payments.Persistence.Abstractions;
using DSDD.Automations.Payments.Persistence.Abstractions.Model.Payers;
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
    
    public async Task<ReportFile> GenerateXlsxAsync(CancellationToken ct)
    {
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
                cell.Value = XLCellValue.FromObject(monthYear);
            });
        
        MonthYear feesMandatoryFrom = new(_options.FromMonth, _options.FromYear);

        int rowNumber = 2;
        foreach (Member member in members)
        {
            Payer payer = (await _payersDao.GetAsync(member.VariableSymbol, ct)) ?? new(member.VariableSymbol);

            (decimal fee, ulong constantSymbol, bool isAnnual) = member.FeePaymentPeriod == MemberFeePaymentPeriod.ANNUALLY
                ? (_options.AnnualyFeeCzk, _options.AnuallyConstantSymbol, true)
                : (_options.MonthlyFeeCzk, _options.MonthlyConstantSymbol, false);

            PayerPaymentsPerMonth payedPerMonth = new(payer, constantSymbol);

            MonthYear enlisted = new(member.Enlisted);

            MonthYear? maybeLastAnnualPayment = null;

            writeRow(
                worksheet.Row(rowNumber),
                cell => cell.Value = XLCellValue.FromObject(member.FirstName),
                cell => cell.Value = XLCellValue.FromObject(member.LastName),
                cell => cell.Value = XLCellValue.FromObject(member.VariableSymbol),
                (cell, monthYear) =>
                {
                    cell.Style.Font.Bold = true;

                    bool thisMonthHasPaymentRequirement = feesMandatoryFrom <= monthYear;
                    if (!thisMonthHasPaymentRequirement)
                        return;

                    bool isEnlistedForThisMonth = enlisted <= monthYear;
                    if (!isEnlistedForThisMonth)
                        return;

                    decimal payedThisMonth = payedPerMonth.Get(monthYear);
                    bool payedEnough = fee <= payedThisMonth;

                    if (payedEnough && isAnnual)
                        maybeLastAnnualPayment = monthYear;

                    if (!payedEnough && isAnnual && maybeLastAnnualPayment is {} lastAnnualPayment)
                    { 
                        long monthsPassed = (monthYear - lastAnnualPayment) 
                                            // Month of the payment should be included: 1.2025 - 2.2025 should be 2
                                            + 1;
                        
                        if (monthsPassed <= 12)
                        {
                            cell.Value = XLCellValue.FromObject(payedThisMonth);
                            cell.Style.Font.FontColor = XLColor.Orange;
                            return;
                        }
                    }
                    
                    cell.Value = XLCellValue.FromObject(payedThisMonth);
                    cell.Style.Font.Bold = true;

                    if (!payedEnough)
                    {
                        cell.Style.Font.FontColor = XLColor.Red;
                        return;
                    }

                    cell.Style.Font.FontColor = XLColor.Green;
                });

            rowNumber++;
        }

        foreach (IXLColumn column in worksheet.ColumnsUsed())
            column.AdjustToContents();
        
        IXLTable table = worksheet.RangeUsed().CreateTable();
        ClosedXmlHelpers.ApplyCommonTableTheme(table);

        table.ShowTotalsRow = true;
        foreach (IXLTableField field in table.Fields.Skip(3))
            field.TotalsRowFunction = XLTotalsRowFunction.Sum;
        IXLRangeRow totalsRow = table.TotalsRow();
        totalsRow.Style.Font.FontColor = XLColor.Black;
        totalsRow.Style.Font.Bold = false;
        
        table
            .Range(2, 3, table.LastRow().RowNumber(), 3)
            .Style
            .Border
            .SetRightBorder(XLBorderStyleValues.Medium);

        Stream stream = ClosedXmlHelpers.SaveToMemory(workbook);

        return ReportFile.FromXlsx("member-fees", stream);
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