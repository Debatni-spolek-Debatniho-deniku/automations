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
        _payers.Setup(p => p.GetAsync(variableSymbol)).ReturnsAsync(payer);

        // Act
        await _sut.UpsertManualPaymentAsync(variableSymbol, paymentReference, constantSymbol, amountCzk, dateTime, description);

        // Assert
        Assert.That(payer.ManualPayments, Has.Count.EqualTo(1));

        if (paymentReference is not null)
            Assert.That(payer.ManualPayments.Single().Reference, Is.EqualTo(paymentReference));

        Assert.That(payer.ManualPayments.Single().ConstantSymbol, Is.EqualTo(constantSymbol));
        Assert.That(payer.ManualPayments.Single().AmountCzk, Is.EqualTo(amountCzk));
        Assert.That(payer.ManualPayments.Single().DateTime, Is.EqualTo(dateTime));
        Assert.That(payer.ManualPayments.Single().Description, Is.EqualTo(description));

        _payers.Verify(_ => _.UpsertAync(payer), Times.Once);
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
            null);

        _payers.Verify(_ => _.UpsertAync(It.Is<Payer>(p => p.VariableSymbol == NEW_PAYER_SYMBOL)));
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
        _payers.Setup(p => p.GetAsync(variableSymbol)).ReturnsAsync(payer);

        // Act
        await _sut.OverrideBankPaymentAsync(variableSymbol, paymentReference, constantSymbol, dateTime, description);

        // Assert
        Assert.That(payment.Overrides.ConstantSymbol, Is.EqualTo(constantSymbol));
        Assert.That(payment.Overrides.DateTime, Is.EqualTo(dateTime));
        Assert.That(payment.Overrides.Description, Is.EqualTo(description));

        _payers.Verify(_ => _.UpsertAync(payer), Times.Once);
    }
    
    [Test]
    public void OverrideBankPaymentAsync_NonExistingPayer()
        => Assert.ThrowsAsync<NullReferenceException>(() => _sut.OverrideBankPaymentAsync(50, "foo", null, null, null));

    [Test]
    public void OverrideBankPaymentAsync_NonExistingPayment()
    {
        // Arrange
        const ulong VARIABLE_SYMBOL = 4;
        const string PAYMENT_REFERENCE = "sss";
        
        Payer payer = new(VARIABLE_SYMBOL);
        payer.BankPayments.Add(new ("bar", PAYMENT_REFERENCE, null, 50, DateTime.Now, null, new(false, null, null, null)));

        _payers.Setup(_ => _.GetAsync(VARIABLE_SYMBOL)).ReturnsAsync(payer);

        // Act and assert
        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.OverrideBankPaymentAsync(VARIABLE_SYMBOL, "other", null, null, null));
    }

    [TestCase(1234567890ul, "ref123")]
    public async Task RemovePaymentAsync(ulong variableSymbol, string paymentReference)
    {
        // Arrange
        ManualPayment manualPayment = new(paymentReference, null, 0, DateTime.MinValue, null);
        BankPayment bankPayment = new(paymentReference, "FOO", null, 10, DateTime.Now, null, new(false, null, null, null));
        Payer payer = new(variableSymbol);
        payer.ManualPayments.Add(manualPayment);
        payer.BankPayments.Add(bankPayment);
        _payers.Setup(p => p.GetAsync(variableSymbol)).ReturnsAsync(payer);

        // Act
        await _sut.RemovePaymentAsync(variableSymbol, paymentReference);

        // Assert
        Assert.That(payer.ManualPayments.Any(mp => mp.Reference == paymentReference), Is.False);
        Assert.That(payer.BankPayments.Single(bp => bp.Reference == paymentReference).Overrides.Hidden, Is.True);
        _payers.Verify(p => p.UpsertAync(payer), Times.Once);
    }

    [Test]
    public void RemovePaymentAsync_NonExistingPayer()
        => Assert.ThrowsAsync<NullReferenceException>(() => _sut.RemovePaymentAsync(50, "foo"));

    [Test]
    public void RemovePaymentAsync_NonExistingPayment()
    {
        // Arrange
        const ulong VARIABLE_SYMBOL = 4;

        Payer payer = new(VARIABLE_SYMBOL);
        _payers.Setup(_ => _.GetAsync(VARIABLE_SYMBOL)).ReturnsAsync(payer);

        // Act and assert
        Assert.ThrowsAsync<IndexOutOfRangeException>(() => _sut.RemovePaymentAsync(VARIABLE_SYMBOL, "payment"));
    }

    private Mock<IPayersDao> _payers;
    private PayerPaymentsService _sut;
}