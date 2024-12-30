namespace DSDD.Automations.Reports.Reports;

public struct ReportFile
{
    public string Name { get; }

    public string ContentType { get; }

    public Stream Content { get; }

    public ReportFile(string name, string contentType, Stream content)
    {
        Name = name;
        ContentType = contentType;
        Content = content;
    }

    /// <summary>
    /// Creates instance representing XLSX file.
    /// </summary>
    public static ReportFile FromXlsx(string name, Stream content)
        => new(
            $"{name}-{DateTime.Now:s}.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            content);
}