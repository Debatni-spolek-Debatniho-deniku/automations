using DSDD.Automations.Payments.Persistence.Abstractions.Model.Payers;
using DSDD.Automations.Reports.Reports.MemberFees;

namespace DSDD.Automations.Reports.Tests.Reports.MemberFees;

public class PayerPaymentsPerMonthTests
{
    [Test]
    public void Get()
    {
        // Setup
        DateTime now = DateTime.Now;
        DateTime monthAgo = now.AddMonths(-1);
        DateTime monthNext = now.AddMonths(1);
        const ulong CONSTANT_SYMBOL = 100;

        Payer payer = new(10);
        payer.BankPayments.Add(new("foo", "bar", 50, 800, monthAgo, null, new(false, null, null, null)));
        payer.BankPayments.Add(new("foo", "bar", CONSTANT_SYMBOL, 120, monthAgo, null, new(false, null, null, null)));
        payer.BankPayments.Add(new("foo", "bar", 50, 100, now, null, new(false, null, null, null)));
        payer.BankPayments.Add(new("foo", "bar", CONSTANT_SYMBOL, 300, now, null, new(false, null, null, null)));
        payer.BankPayments.Add(new("foo", "bar", CONSTANT_SYMBOL, 10, now, null, new(false, null, monthAgo, null)));
        payer.BankPayments.Add(new("foo", "bar", CONSTANT_SYMBOL, 30, now, null, new(false, 50, null, null)));
        payer.BankPayments.Add(new("foo", "bar", CONSTANT_SYMBOL, 180, now, null, new(true, null, null, null)));
        payer.BankPayments.Add(new("foo", "bar", 50, 810, monthNext, null, new(false, null, null, null)));
        payer.BankPayments.Add(new("foo", "bar", CONSTANT_SYMBOL, 170, monthNext, null, new(false, null, null, null)));

        payer.ManualPayments.Add(new("foo", 50, 200, monthAgo, null));
        payer.ManualPayments.Add(new("foo", CONSTANT_SYMBOL, 200, monthAgo, null));
        payer.ManualPayments.Add(new("foo", 50, 550, now, null));
        payer.ManualPayments.Add(new("foo", CONSTANT_SYMBOL, 120, now, null));
        payer.ManualPayments.Add(new("foo", 50, 200, monthNext, null));
        payer.ManualPayments.Add(new("foo", CONSTANT_SYMBOL, 200, monthNext, null));

        PayerPaymentsPerMonth sut = new(payer, CONSTANT_SYMBOL);

        // Act
        decimal resultMonthAgo = sut.Get(new(monthAgo));
        decimal resultNow = sut.Get(new(now));
        decimal resultMonthNext = sut.Get(new(monthNext));

        // Assert
        Assert.That(resultMonthAgo, Is.EqualTo(330));
        Assert.That(resultNow, Is.EqualTo(420));
        Assert.That(resultMonthNext, Is.EqualTo(370));
    }
}