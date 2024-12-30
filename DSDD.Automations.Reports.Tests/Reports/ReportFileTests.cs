using DSDD.Automations.Reports.Reports;

namespace DSDD.Automations.Reports.Tests.Reports;

public class ReportFileTests
{
    [Test]
    public void FromXlsx()
    {
        using Stream stream = new MemoryStream();
        const string NAME = "foo";

        ReportFile file = ReportFile.FromXlsx(NAME, stream);

        Assert.That(file, Has.Property(nameof(ReportFile.Content)).EqualTo(stream));
        Assert.That(file, Has.Property(nameof(ReportFile.ContentType)).EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
        Assert.That(file, Has.Property(nameof(ReportFile.Name)).Match(NAME + "-\\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}\\.xlsx"));
    }
}