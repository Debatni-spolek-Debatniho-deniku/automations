namespace DSDD.Automations.Reports.Reports.MemberFees;

public struct MonthYear
{
    public ushort Month { get; set; }

    public ushort Year { get; set; }

    public MonthYear(ushort month, ushort year)
    {
        Month = month;
        Year = year;
    }

    public MonthYear(DateOnly date) : this((ushort)date.Month, (ushort)date.Year)
    {
    }

    public MonthYear(DateTime dateTime) : this((ushort)dateTime.Month, (ushort)dateTime.Year)
    {
    }

    public override string ToString()
        => new DateOnly(Year, Month, 1).ToString("MMMM yyyy");

    public override int GetHashCode()
        => Month + Year;

    public override bool Equals(object? obj)
        => obj is MonthYear my && my == this;

    public static bool operator ==(MonthYear left, MonthYear right)
        => left.Month == right.Month && left.Year == right.Year;

    public static bool operator <(MonthYear left, MonthYear right)
    {
        if (left.Year < right.Year)
            return true;
        if (left.Year > right.Year)
            return false;

        return left.Month < right.Month;
    }

    public static bool operator !=(MonthYear left, MonthYear right)
        => !(left == right);

    public static bool operator <=(MonthYear left, MonthYear right)
        => left < right || left == right;

    public static bool operator >(MonthYear left, MonthYear right)
        => !(left < right) && left != right;

    public static bool operator >=(MonthYear left, MonthYear right)
        => left > right || left == right;

    public static long operator -(MonthYear left, MonthYear right)
    {
        if (left < right)
            return -1 * (right - left);
        return (left.Year - right.Year) * 12 + (left.Month - right.Month);
    }
}