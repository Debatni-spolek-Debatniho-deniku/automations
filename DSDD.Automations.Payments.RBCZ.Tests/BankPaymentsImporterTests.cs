using System.Collections;
using DSDD.Automations.Payments.Helpers;
using DSDD.Automations.Payments.Model;
using DSDD.Automations.Payments.RBCZ.PremiumApi;
using Microsoft.Extensions.Logging;

namespace DSDD.Automations.Payments.RBCZ.Tests;

internal class BankPaymentsImporterTests
{
    [SetUp]
    public void SetUp()
    {
        _payers = new();
        _premiumApiClient = new();
        _numericSymbolParser = new();
        _numericSymbolParser.Setup(_ => _.Parse(It.IsAny<string>())).Returns((string value) => ulong.Parse(value));

        _fxRates = new();

        _sut = new(
            Mock.Of<ILogger<BankPaymentsImporter>>(),
            _payers.Object,
            _premiumApiClient.Object,
            _numericSymbolParser.Object,
            _fxRates.Object);
    }
    
    [Test]
    public async Task ImportAsync_Czk()
    {
        // Setup
        DateTime NOW = DateTime.Now;
        
        const string CZK = "CZK";

        const ulong VARIABLE_SYMBOL_1 = 1;
        const string PAYER_1_TRANSACTION_1_REFERENCE = "tran1";
        const double PAYER_1_TRANSACTION_1_AMOUNT = 90;
        const string PAYER_1_TRANSACTION_1_ACCOUNT_PREFIX = "123";
        const string PAYER_1_TRANSACTION_1_ACCOUNTNUMBER = "555222";
        const string PAYER_1_TRANSACTION_1_BANKCODE = "200";
        const string PAYER_1_TRANSACTION_1_UNSTRUCTURED = "description";

        const string PAYER_1_TRANSACTION_2_REFERENCE = "tran2";
        const string PAYER_1_TRANSACTION_2_COUNTERPARTY = "foo";

        Payer payer1 = new(VARIABLE_SYMBOL_1);
        BankPaymentOverrides payer1transaction2overrides = new(false, null, null, null);
        payer1.BankPayments.Add(new(
            PAYER_1_TRANSACTION_2_REFERENCE,
            PAYER_1_TRANSACTION_2_COUNTERPARTY,
            50,
            5000,
            NOW,
            null,
            payer1transaction2overrides));

        Transaction payer1transaction1 = new()
        {
            EntryReference = PAYER_1_TRANSACTION_1_REFERENCE,
            BookingDate = NOW,
            Amount = new()
            {
                Currency = CZK,
                Value = PAYER_1_TRANSACTION_1_AMOUNT
            },
            EntryDetails = new()
            {
                TransactionDetails = new()
                {
                    RemittanceInformation = new()
                    {
                        Unstructured = PAYER_1_TRANSACTION_1_UNSTRUCTURED,
                        CreditorReferenceInformation = new()
                        {
                            Variable = VARIABLE_SYMBOL_1.ToString(),
                        },
                    },
                    RelatedParties = new()
                    {
                        CounterParty = new()
                        {
                            OrganisationIdentification = new()
                            {
                                BankCode = PAYER_1_TRANSACTION_1_BANKCODE
                            },
                            Account = new()
                            {
                                AccountNumber = PAYER_1_TRANSACTION_1_ACCOUNTNUMBER,
                                AccountNumberPrefix = PAYER_1_TRANSACTION_1_ACCOUNT_PREFIX
                            }
                        }
                    },
                }
            }
        };

        Transaction payer1transaction2 = new()
        {
            EntryReference = PAYER_1_TRANSACTION_2_REFERENCE,
            BookingDate = NOW,
            EntryDetails = new()
            {
                TransactionDetails = new()
                {
                    RemittanceInformation = new()
                    {
                        CreditorReferenceInformation = new()
                        {
                            Variable = VARIABLE_SYMBOL_1.ToString()
                        }
                    },
                    RelatedParties = new()
                    {
                        CounterParty = new()
                        {
                            Account = new()
                            {
                                Iban = "bar"
                            }
                        }
                    }
                }
            }
        };

        const ulong VARIABLE_SYMBOL_2 = 2;
        const ulong CONSTANT_SYMBOL_2 = 100;
        const string PAYER_2_TRANSACTION_1_REFERENCE = "tran3";
        const double PAYER_2_TRANSACTION_1_AMOUNT = 90;
        const string PAYER_2_TRANSACTION_1_ACCOUNT_IBAN = "iban";

        Transaction payer2transaction1 = new()
        {
            EntryReference = PAYER_2_TRANSACTION_1_REFERENCE,
            BookingDate = NOW,
            Amount = new()
            {
                Currency = CZK,
                Value = PAYER_2_TRANSACTION_1_AMOUNT
            },
            EntryDetails = new()
            {
                TransactionDetails = new()
                {
                    RemittanceInformation = new()
                    {
                        CreditorReferenceInformation = new()
                        {
                            Variable = VARIABLE_SYMBOL_2.ToString(),
                            Constant = CONSTANT_SYMBOL_2.ToString()
                        }
                    },
                    RelatedParties = new()
                    {
                        CounterParty = new()
                        {
                            Account = new()
                            {
                                Iban = PAYER_2_TRANSACTION_1_ACCOUNT_IBAN
                            }
                        }
                    }
                }
            }
        };

        _payers.Setup(_ => _.GetAsync(VARIABLE_SYMBOL_1, default)).ReturnsAsync(payer1);
        _premiumApiClient
            .Setup(_ => _.GetLast90DaysTransactionsAsync(default))
            .Returns(new[] { payer1transaction1, payer1transaction2, payer2transaction1 }.ToAsyncEnumerable());

        List<Payer> results = new List<Payer>();
        _payers.Setup(_ => _.UpsertAync(Capture.In(results), default));

        // Act
        await _sut.ImportAsync(default);

        // Assert
        _payers.Verify(_ => _.GetAsync(It.IsAny<ulong>(), default), Times.Exactly(2));
        _payers.Verify(_ => _.UpsertAync(It.IsAny<Payer>(), default), Times.Exactly(2));

        Payer result1 = results[0];
        Assert.That(result1, Has
            .Property(nameof(Payer.VariableSymbol)).EqualTo(VARIABLE_SYMBOL_1)
            .And
            .Property(nameof(Payer.BankPayments)).Count.EqualTo(2));
        // Already existing
        BankPayment result1payment1 = result1.BankPayments.First();
        Assert.That(result1payment1, Has.Property(nameof(BankPayment.CounterpartyAccountNumber)).EqualTo(PAYER_1_TRANSACTION_2_COUNTERPARTY));
        // Newly added 
        BankPayment result1payment2 = result1.BankPayments.Last();
        Assert.That(result1payment2, Has
            .Property(nameof(BankPayment.Reference)).EqualTo(PAYER_1_TRANSACTION_1_REFERENCE)
            .And
            .Property(nameof(BankPayment.AmountCzk)).EqualTo(PAYER_1_TRANSACTION_1_AMOUNT)
            .And
            .Property(nameof(BankPayment.ConstantSymbol)).Null
            .And
            .Property(nameof(BankPayment.DateTime)).EqualTo(NOW)
            .And
            .Property(nameof(BankPayment.Description)).EqualTo(PAYER_1_TRANSACTION_1_UNSTRUCTURED)
            .And
            .Property(nameof(BankPayment.CounterpartyAccountNumber)).EqualTo($"{PAYER_1_TRANSACTION_1_ACCOUNT_PREFIX}-{PAYER_1_TRANSACTION_1_ACCOUNTNUMBER}/{PAYER_1_TRANSACTION_1_BANKCODE}"));

        Payer result2 = results[1];
        Assert.That(result2, Has
            .Property(nameof(Payer.VariableSymbol)).EqualTo(VARIABLE_SYMBOL_2)
            .And
            .Property(nameof(Payer.BankPayments)).Count.EqualTo(1));
        // Newly added 
        BankPayment result2payment1 = result2.BankPayments.Single();
        Assert.That(result2payment1, Has
            .Property(nameof(BankPayment.Reference)).EqualTo(PAYER_2_TRANSACTION_1_REFERENCE)
            .And
            .Property(nameof(BankPayment.AmountCzk)).EqualTo(PAYER_2_TRANSACTION_1_AMOUNT)
            .And
            .Property(nameof(BankPayment.ConstantSymbol)).EqualTo(CONSTANT_SYMBOL_2)
            .And
            .Property(nameof(BankPayment.DateTime)).EqualTo(NOW)
            .And
            .Property(nameof(BankPayment.Description)).Null
            .And
            .Property(nameof(BankPayment.CounterpartyAccountNumber)).EqualTo(PAYER_2_TRANSACTION_1_ACCOUNT_IBAN));
    }

    [Test]
    public async Task ImportAsync_Eur()
    {
        // Setup
        DateTime NOW = DateTime.Now;
        
        const string EUR = "EUR";

        const ulong VARIABLE_SYMBOL = 1;
        const string REFERENCE = "tran1";
        const double AMOUNT = 100;
        const decimal AMOUNT_CZK = 2100; 
        const string ACCOUNTNUMBER = "111";
        const string BANKCODE = "5000";

        _fxRates.Setup(_ => _.ToCzkAsync(It.IsAny<decimal>(), EUR, default)).ReturnsAsync(AMOUNT_CZK);

        Transaction transaction = new()
        {
            EntryReference = REFERENCE,
            BookingDate = NOW,
            Amount = new()
            {
                Currency = EUR,
                Value = AMOUNT
            },
            EntryDetails = new()
            {
                TransactionDetails = new()
                {
                    RemittanceInformation = new()
                    {
                        CreditorReferenceInformation = new()
                        {
                            Variable = VARIABLE_SYMBOL.ToString(),
                        },
                    },
                    RelatedParties = new()
                    {
                        CounterParty = new()
                        {
                            OrganisationIdentification = new()
                            {
                                BankCode = BANKCODE
                            },
                            Account = new()
                            {
                                AccountNumber = ACCOUNTNUMBER
                            }
                        }
                    },
                }
            }
        };

        _premiumApiClient
            .Setup(_ => _.GetLast90DaysTransactionsAsync(default))
            .Returns(new[] { transaction }.ToAsyncEnumerable());

        List<Payer> results = new List<Payer>();
        _payers.Setup(_ => _.UpsertAync(Capture.In(results), default));

        // Act
        await _sut.ImportAsync(default);

        // Assert
        _fxRates.Verify(_ => _.ToCzkAsync((decimal)AMOUNT, EUR, default), Times.Once);

        _payers.Verify(_ => _.GetAsync(It.IsAny<ulong>(), default), Times.Exactly(1));
        _payers.Verify(_ => _.UpsertAync(It.IsAny<Payer>(), default), Times.Exactly(1));

        Assert.That(results, Has.Count.EqualTo(1));

        Payer result = results[0];
        Assert.That(result, Has.Property(nameof(Payer.BankPayments)).Count.EqualTo(1));

        BankPayment resultPayment = result.BankPayments.First();
        Assert.That(resultPayment, Has
            .Property(nameof(BankPayment.AmountCzk)).EqualTo(AMOUNT_CZK)
            .And
            .Property(nameof(BankPayment.CounterpartyAccountNumber)).EqualTo($"{ACCOUNTNUMBER}/{BANKCODE}"));
    }

    [Test]
    public async Task ImportAsync_MissingRemittanceInformation()
    {
        // Setup
        DateTime NOW = DateTime.Now;

        const string CZK = "CZK";
        
        const string TRANSACTION_REFERENCE = "tran1";
        const double TRANSACTION_AMOUNT = 90;
        const string TRANSACTION_ACCOUNT_PREFIX = "123";
        const string TRANSACTION_ACCOUNTNUMBER = "555222";
        const string TRANSACTION_BANKCODE = "200";

        Transaction transaction = new()
        {
            EntryReference = TRANSACTION_REFERENCE,
            BookingDate = NOW,
            Amount = new()
            {
                Currency = CZK,
                Value = TRANSACTION_AMOUNT
            },
            EntryDetails = new()
            {
                TransactionDetails = new()
                {
                    RelatedParties = new()
                    {
                        CounterParty = new()
                        {
                            OrganisationIdentification = new()
                            {
                                BankCode = TRANSACTION_BANKCODE
                            },
                            Account = new()
                            {
                                AccountNumber = TRANSACTION_ACCOUNTNUMBER,
                                AccountNumberPrefix = TRANSACTION_ACCOUNT_PREFIX
                            }
                        }
                    },
                }
            }
        };

        _premiumApiClient
            .Setup(_ => _.GetLast90DaysTransactionsAsync(default))
            .Returns(new[] { transaction }.ToAsyncEnumerable());

        // Act
        await _sut.ImportAsync(default);

        // Assert
        // Payers and transactions are mapped using variable symbol. 
        // Missing RemittanceInformation (and var. smybol) should prevent creating a new Payer (or attaching transaction to an existing one).
        _payers.Verify(_ => _.UpsertAync(It.IsAny<Payer>(), default), Times.Never);
    }

    public static IEnumerable ImportAsync_Skips_Source
    {
        get
        {
            // Missing varaible symbol
            Transaction transaction = new()
            {
                EntryDetails = new()
                {
                    TransactionDetails = new()
                    {
                        RemittanceInformation = new()
                        {
                            CreditorReferenceInformation = new()
                            {
                                Variable = null
                            },
                        },
                        RelatedParties = new()
                        {
                            CounterParty = new()
                            {
                                Account = new()
                                {
                                    Iban = "asdad"
                                }
                            }
                        }
                    }
                },
                BookingDate = DateTime.Now,
            };

            yield return new object[] { transaction };

            transaction = new()
            {
                EntryDetails = new()
                {
                    TransactionDetails = new()
                    {
                        RemittanceInformation = new()
                        {
                            CreditorReferenceInformation = new()
                            {
                                Variable = "100"
                            },
                        },
                        RelatedParties = new()
                        {
                            CounterParty = new()
                            {
                                Account = new()
                                {
                                    Iban = "asdad"
                                }
                            }
                        }
                    }
                },
                BookingDate = null,
            };

            yield return new object[] { transaction };

            transaction = new()
            {
                EntryDetails = new()
                {
                    TransactionDetails = new()
                    {
                        RemittanceInformation = new()
                        {
                            CreditorReferenceInformation = new()
                            {
                                Variable = "100"
                            },
                        },
                        RelatedParties = new()
                        {
                            CounterParty = new()
                            {
                                Account = new()
                                {
                                    Iban = null
                                }
                            }
                        }
                    }
                },
                BookingDate = DateTime.Now,
            };

            yield return new object[] { transaction };
        }
    }

    [TestCaseSource(nameof(ImportAsync_Skips_Source))]
    public async Task ImportAsync_Skips(Transaction transaction)
    {
        _premiumApiClient
            .Setup(_ => _.GetLast90DaysTransactionsAsync(default))
            .Returns(new[] { transaction }.ToAsyncEnumerable());

        // Act
        await _sut.ImportAsync(default);

        // Assert
        _payers.Verify(_ => _.GetAsync(It.IsAny<ulong>(), default), Times.Never);
        _payers.Verify(_ => _.UpsertAync(It.IsAny<Payer>(), default), Times.Never);
    }

    private Mock<IPayersDao> _payers = null!;
    private Mock<IPremiumApiClient> _premiumApiClient = null!;
    private Mock<INumericSymbolParser> _numericSymbolParser = null!;
    private Mock<IFxRates> _fxRates = null!;
    private BankPaymentsImporter _sut = null!;
}