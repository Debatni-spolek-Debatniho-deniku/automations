using System.Collections;
using DSDD.Automations.Reports.Reports.MemberFees;

namespace DSDD.Automations.Reports.Tests.Reports.MemberFees;

public class MonthYearTests
{
    public static IEnumerable Equals_Srouce
    {
        get
        {
            MonthYear left = new(10, 2024);
            MonthYear right = new(10, 2024);
            bool expected = true;

            yield return new object[] { left, right, expected };

            left = new(9, 2024);
            right = new(10, 2024);
            expected = false;

            yield return new object[] { left, right, expected };

            left = new(10, 2023);
            right = new(10, 2024);
            expected = false;

            yield return new object[] { left, right, expected };
        }
    }

    [TestCaseSource(nameof(Equals_Srouce))]
    public void Equals(MonthYear left, MonthYear right, bool expected)
    {
        Assert.That(left == right, Is.EqualTo(expected));
        Assert.That(left.Equals(right), Is.EqualTo(expected));
    }

    public static IEnumerable NotEquals_Source
    {
        get
        {
            MonthYear left = new(10, 2024);
            MonthYear right = new(10, 2024);
            bool expected = false;

            yield return new object[] { left, right, expected };

            left = new(9, 2024);
            right = new(10, 2024);
            expected = true;

            yield return new object[] { left, right, expected };
        }
    }

    [TestCaseSource(nameof(NotEquals_Source))]
    public void NotEquals(MonthYear left, MonthYear right, bool expected)
    {
        Assert.That(left != right, Is.EqualTo(expected));
    }

    public static IEnumerable Greater_Source
    {
        get
        {
            MonthYear left = new(9, 2024);
            MonthYear right = new(10, 2024);
            bool expected = false;

            yield return new object[] { left, right, expected };

            left = new(10, 2024);
            right = new(10, 2024);
            expected = false;

            yield return new object[] { left, right, expected };

            left = new(11, 2024);
            right = new(10, 2024);
            expected = true;

            yield return new object[] { left, right, expected };
        }
    }

    [TestCaseSource(nameof(Greater_Source))]
    public void Greater(MonthYear left, MonthYear right, bool expected)
    {
        Assert.That(left > right, Is.EqualTo(expected));
    }

    public static IEnumerable GreaterOrEqual_Source
    {
        get
        {
            MonthYear left = new(9, 2024);
            MonthYear right = new(10, 2024);
            bool expected = false;

            yield return new object[] { left, right, expected };

            left = new(10, 2024);
            right = new(10, 2024);
            expected = true;

            yield return new object[] { left, right, expected };

            left = new(11, 2024);
            right = new(10, 2024);
            expected = true;

            yield return new object[] { left, right, expected };
        }
    }

    [TestCaseSource(nameof(GreaterOrEqual_Source))]
    public void GreaterOrEqual(MonthYear left, MonthYear right, bool expected)
    {
        Assert.That(left >= right, Is.EqualTo(expected));
    }

    public static IEnumerable Lower_Source
    {
        get
        {
            MonthYear left = new(9, 2024);
            MonthYear right = new(10, 2024);
            bool expected = true;

            yield return new object[] { left, right, expected };

            left = new(10, 2024);
            right = new(10, 2024);
            expected = false;

            yield return new object[] { left, right, expected };

            left = new(11, 2024);
            right = new(10, 2024);
            expected = false;

            yield return new object[] { left, right, expected };
        }
    }

    [TestCaseSource(nameof(Lower_Source))]
    public void Lower(MonthYear left, MonthYear right, bool expected)
    {
        Assert.That(left < right, Is.EqualTo(expected));
    }

    public static IEnumerable LowerOrEqual_Source
    {
        get
        {
            MonthYear left = new(9, 2024);
            MonthYear right = new(10, 2024);
            bool expected = true;

            yield return new object[] { left, right, expected };

            left = new(10, 2024);
            right = new(10, 2024);
            expected = true;

            yield return new object[] { left, right, expected };

            left = new(11, 2024);
            right = new(10, 2024);
            expected = false;

            yield return new object[] { left, right, expected };
        }
    }

    [TestCaseSource(nameof(LowerOrEqual_Source))]
    public void LowerOrEqual(MonthYear left, MonthYear right, bool expected)
    {
        Assert.That(left <= right, Is.EqualTo(expected));
    }
}