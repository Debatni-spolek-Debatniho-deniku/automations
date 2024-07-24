using DSDD.Automations.Payments.RBCZ.PremiumApi;

namespace DSDD.Automations.Payments.RBCZ.Tests;

public class FxRatesTests
{
    [SetUp]
    public void SetUp()
    {
        _apiClient = new();

        _sut = new(_apiClient.Object);
    }

    [TestCase(1, "USD", 21)]
    [TestCase(0.5, "USD", 10.5)]
    [TestCase(2, "EUR", 52)]
    [TestCase(10, "ZMW", 9)]
    public async Task ToCzkAsync(decimal value, string currency, decimal expected)
    {
        // Setup
        _apiClient.Setup(_ => _.GetFxRateAsync("USD", default)).ReturnsAsync(21m);
        _apiClient.Setup(_ => _.GetFxRateAsync("EUR", default)).ReturnsAsync(26m);
        _apiClient.Setup(_ => _.GetFxRateAsync("ZMW", default)).ReturnsAsync(0.9m);


        // Act
        decimal result = await _sut.ToCzkAsync(value, currency, default);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public async Task ToCzkAsync_Cache()
    {
        // Setup
        _apiClient.Setup(_ => _.GetFxRateAsync("USD", default)).ReturnsAsync(21m);


        // Act
        Task task1 =  _sut.ToCzkAsync(1, "USD", default);
        Task task2 =  _sut.ToCzkAsync(80, "USD", default);
        Task task3 =  _sut.ToCzkAsync(600, "USD", default);

        await Task.WhenAll(task1, task2, task3);

        // Assert
        _apiClient.Verify(_ => _.GetFxRateAsync("USD", default), Times.Once);
    }


    [Test]
    public async Task ToCzkAsync_UknownCurrency()
        => Assert.ThrowsAsync<InvalidOperationException>(() => _sut.ToCzkAsync(50, "USD", default));

    private Mock<IApiClient> _apiClient = null!;
    private FxRates _sut = null!;
}