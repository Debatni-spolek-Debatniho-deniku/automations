using DSDD.Automations.Payments.Model;
using DSDD.Automations.Payments.Payments;

namespace DSDD.Automations.Payments.Tests.Payments;

public class PayerPaymentsServiceTests
{
    [SetUp]
    public void Setup()
    {
        _payers = new Mock<IPayersDao>();
        _sut = new PayerPaymentsService(_payers.Object);
    }

    [TestCase(1234567890ul, null, null, 100.50, "2023-07-17", null)]
    [TestCase(1234567890ul, "ref123", 1234ul, 200.75, "2023-07-17", "Payment description")]
    public async Task UpsertManualPaymentAsync(
        ulong variableSymbol,
        string? paymentReference, 
        ulong? constantSymbol,
        decimal amountCzk, 
        DateTime dateTime, 
        string? description)
    {
        // Arrange
        Payer payer = new Payer(variableSymbol);
        _payers.Setup(p => p.GetAsync(variableSymbol, default)).ReturnsAsync(payer);

        // Act
        await _sut.UpsertManualPaymentAsync(variableSymbol, paymentReference, constantSymbol, amountCzk, dateTime, description, default);

        // Assert
        Assert.That(payer.ManualPayments, Has.Count.EqualTo(1));

        if (paymentReference is not null)
            Assert.That(payer.ManualPayments.Single().Reference, Is.EqualTo(paymentReference));

        Assert.That(payer.ManualPayments.Single().ConstantSymbol, Is.EqualTo(constantSymbol));
        Assert.That(payer.ManualPayments.Single().AmountCzk, Is.EqualTo(amountCzk));
        Assert.That(payer.ManualPayments.Single().DateTime, Is.EqualTo(dateTime));
        Assert.That(payer.ManualPayments.Single().Description, Is.EqualTo(description));

        _payers.Verify(_ => _.GetAsync(variableSymbol, default), Times.Once);
        _payers.Verify(_ => _.UpsertAync(payer, default), Times.Once);
    }

    [Test]
    public async Task UpsertManualPaymentAsync_NonExistingPayer()
    {
        const ulong NEW_PAYER_SYMBOL = 50;

        await _sut.UpsertManualPaymentAsync(
            NEW_PAYER_SYMBOL,
            null,
            50,
            100,
            DateTime.Now,
            null,
            default);

        _payers.Verify(_ => _.UpsertAync(It.Is<Payer>(p => p.VariableSymbol == NEW_PAYER_SYMBOL), default));
    }

    [TestCase(1234567890ul, "ref123", 1234ul, "2023-07-17", "Updated description")]
    public async Task OverrideBankPaymentAsync(
        ulong variableSymbol, 
        string paymentReference,
        ulong? constantSymbol, 
        DateTime? dateTime,
        string? description)
    {
        // Arrange
        BankPayment payment = new(
            paymentReference,
            "FOO",
            0ul,
            100,
            new DateTime(1990, 12, 31, 12, 00, 00),
            "foooo",
            new(false, null, null, null));

        var payer = new Payer(variableSymbol);
        payer.BankPayments.Add(payment);
        _payers.Setup(p => p.GetAsync(variableSymbol, default)).ReturnsAsync(payer);

        // Act
        await _sut.OverrideBankPaymentAsync(variableSymbol, paymentReference, constantSymbol, dateTime, description, default);

        // Assert
        Assert.That(payment.Overrides.ConstantSymbol, Is.EqualTo(constantSymbol));
        Assert.That(payment.Overrides.DateTime, Is.EqualTo(dateTime));
        Assert.That(payment.Overrides.Description, Is.EqualTo(description));

        _payers.Verify(_ => _.GetAsync(variableSymbol, default), Times.Once);
        _payers.Verify(_ => _.UpsertAync(payer, default), Times.Once);
    }
    
    [Test]
    public void OverrideBankPaymentAsync_NonExistingPayer()
        => Assert.ThrowsAsync<NullReferenceException>(() => _sut.OverrideBankPaymentAsync(50, "foo", null, null, null, default));

    [Test]
    public void OverrideBankPaymentAsync_NonExistingPayment()
    {
        // Arrange
        const ulong VARIABLE_SYMBOL = 4;
        const string PAYMENT_REFERENCE = "sss";
        
        Payer payer = new(VARIABLE_SYMBOL);
        payer.BankPayments.Add(new ("bar", PAYMENT_REFERENCE, null, 50, DateTime.Now, null, new(false, null, null, null)));

        _payers.Setup(_ => _.GetAsync(VARIABLE_SYMBOL, default)).ReturnsAsync(payer);

        // Act and assert
        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.OverrideBankPaymentAsync(VARIABLE_SYMBOL, "other", null, null, null, default));
    }

    [Test]
    public async Task RemovePaymentAsync_BankPayment()
    {
        const ulong VARIABLE_SYMBOL = 5641616;
        const string REFERENCE = "ref12";

        // Arrange
        BankPayment bankPayment = new(REFERENCE, "FOO", null, 10, DateTime.Now, null, new(false, null, null, null));
        Payer payer = new(VARIABLE_SYMBOL);
        payer.BankPayments.Add(bankPayment);
        _payers.Setup(p => p.GetAsync(VARIABLE_SYMBOL, default)).ReturnsAsync(payer);

        // Act
        await _sut.RemovePaymentAsync(VARIABLE_SYMBOL, REFERENCE, default);

        // Assert
        Assert.That(payer.BankPayments.Single(bp => bp.Reference == REFERENCE).Overrides.Removed, Is.True);

        _payers.Verify(_ => _.GetAsync(VARIABLE_SYMBOL, default), Times.Once);
        _payers.Verify(_ => _.UpsertAync(payer, default), Times.Once);
    }

    [Test]
    public async Task RemovePaymentAsync_ManualPayment()
    {
        const ulong VARIABLE_SYMBOL = 5641616;
        const string REFERENCE = "ref12";

        // Arrange
        ManualPayment manualPayment = new(REFERENCE, null, 0, DateTime.MinValue, null);
        Payer payer = new(VARIABLE_SYMBOL);
        payer.ManualPayments.Add(manualPayment);
        _payers.Setup(p => p.GetAsync(VARIABLE_SYMBOL, default)).ReturnsAsync(payer);

        // Act
        await _sut.RemovePaymentAsync(VARIABLE_SYMBOL, REFERENCE, default);

        // Assert
        Assert.That(payer.ManualPayments.Any(mp => mp.Reference == REFERENCE), Is.False);

        _payers.Verify(_ => _.GetAsync(VARIABLE_SYMBOL, default), Times.Once);
        _payers.Verify(_ => _.UpsertAync(payer, default), Times.Once);
    }

    [Test]
    public void RemovePaymentAsync_NonExistingPayer()
        => Assert.ThrowsAsync<NullReferenceException>(() => _sut.RemovePaymentAsync(50, "foo", default));

    [Test]
    public void RemovePaymentAsync_NonExistingPayment()
    {
        // Arrange
        const ulong VARIABLE_SYMBOL = 4;

        Payer payer = new(VARIABLE_SYMBOL);
        _payers.Setup(_ => _.GetAsync(VARIABLE_SYMBOL, default)).ReturnsAsync(payer);

        // Act and assert
        Assert.ThrowsAsync<NullReferenceException>(() => _sut.RemovePaymentAsync(VARIABLE_SYMBOL, "payment", default));
    }

    [Test]
    public async Task RestorePaymentAsync()
    {
        // Arrange
        const ulong VARIABLE_SYMBOL = 50;
        const string REFERENCE = "FOO";

        BankPayment bankPayment = new(REFERENCE, "FOO", null, 10, DateTime.Now, null, new(true, null, null, null));
        Payer payer = new(VARIABLE_SYMBOL);
        payer.BankPayments.Add(bankPayment);
        _payers.Setup(_ => _.GetAsync(VARIABLE_SYMBOL, default)).ReturnsAsync(payer);

        // Act
        await _sut.RestorePaymentAsync(VARIABLE_SYMBOL, REFERENCE, default);

        // Verify
        Assert.That(bankPayment.Overrides.Removed, Is.False);

        _payers.Verify(_ => _.GetAsync(VARIABLE_SYMBOL, default), Times.Once);
        _payers.Verify(_ => _.UpsertAync(payer, default), Times.Once);
    }

    [Test]
    public void RestorePaymentAsync_NonExistingPayer()
        => Assert.ThrowsAsync<NullReferenceException>(() => _sut.RestorePaymentAsync(50, "foo", default));
    
    [Test]
    public void RestorePaymentAsync_NonExistingPayment()
    {
        // Arrange
        const ulong VARIABLE_SYMBOL = 4;

        Payer payer = new(VARIABLE_SYMBOL);
        _payers.Setup(_ => _.GetAsync(VARIABLE_SYMBOL, default)).ReturnsAsync(payer);

        // Act and assert
        Assert.ThrowsAsync<InvalidOperationException>(() => _sut.RestorePaymentAsync(VARIABLE_SYMBOL, "payment", default));
    }

    [Test]
    public void RestorePaymentAsync_ManualPayment()
    {
        // Arrange
        const ulong VARIABLE_SYMBOL = 4;
        const string REFERENCE = "payment";

        ManualPayment payment = new(REFERENCE, null, 50, DateTime.Now, null);

        Payer payer = new(VARIABLE_SYMBOL);
        payer.ManualPayments.Add(payment);
        _payers.Setup(_ => _.GetAsync(VARIABLE_SYMBOL, default)).ReturnsAsync(payer);

        // Act and assert
        Assert.ThrowsAsync<InvalidOperationException>(() => _sut.RestorePaymentAsync(VARIABLE_SYMBOL, REFERENCE, default));
    }

    private Mock<IPayersDao> _payers;
    private PayerPaymentsService _sut;
}