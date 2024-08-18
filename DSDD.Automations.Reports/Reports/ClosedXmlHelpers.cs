using System.Reflection;
using ClosedXML.Excel;

namespace DSDD.Automations.Reports.Reports;

public static class ClosedXmlHelpers
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ConfigurationAttribute : Attribute
    {
        public string Name { get; }
        
        public bool IsCzk { get; init; }

        public XLTotalsRowFunction TotalsFunction { get; init; }

        public ConfigurationAttribute(string name)
        {
            Name = name;
        }
    }

    public static MemoryStream SaveToMemory(XLWorkbook workbook)
    {
        MemoryStream result = new();
        workbook.SaveAs(result);
        result.Seek(0, SeekOrigin.Begin);

        return result;
    }

    public static void ApplyCzkFormat(IXLRangeBase range)
        => range.Style.NumberFormat.Format = "# ##0.00 \"CZK\"";

    public static void ApplyCommonTableTheme(IXLTable table)
        => table.Theme = XLTableTheme.TableStyleMedium18;

    public static MemoryStream ToSingleTableWorkbook<T>(IEnumerable<T> data)
    {
        using XLWorkbook workbook = new();
        IXLWorksheet worksheet = workbook.AddWorksheet();
        IXLTable table = worksheet.FirstCell().InsertTable(data);
        table.Theme = XLTableTheme.TableStyleMedium18;

        List<IXLColumn> dateTimeColumns = new();

        foreach (PropertyInfo property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            IXLTableField field = table.Field(property.Name);

            if (property.GetCustomAttribute<ConfigurationAttribute>() is not {} configuration)
                continue;
            
            field.Name = configuration.Name;

            if (configuration.IsCzk)
            {
                AssertDecimal(property);

                ApplyCzkFormat(field.Column);
            }

            if (configuration.TotalsFunction != XLTotalsRowFunction.None)
            {
                AssertDecimal(property);

                table.ShowTotalsRow = true;
                field.TotalsRowFunction = configuration.TotalsFunction;
            }

            if (property.PropertyType == typeof(DateTime))
                dateTimeColumns.Add(field.Column.WorksheetColumn());
        }

        foreach (IXLRangeColumn column in table.ColumnsUsed())
            column.WorksheetColumn().AdjustToContents();

        // Datetime is too narrow when auto adjusted and must be expanded.
        foreach (IXLColumn dateTimeColumn in dateTimeColumns)
            dateTimeColumn.Width += 2;

        return SaveToMemory(workbook);

        void AssertDecimal(PropertyInfo property)
        {
            if (property.PropertyType != typeof(decimal))
                throw new InvalidOperationException($"Property {property.Name} must be decimal!");
        }
    }
}