using Common.Strategies;
using Domain.Enums;
using Domain.Requests;
using Infrastructure.Entities;

namespace Vanguard.Tests.Common;

public class PaymentStrategyTests
{
    [Fact]
    public void GivenAccountWithBacs_WhenUsingBacsPaymentStrategy_ThenCanProcess()
    {
        // Arrange
        var strategy = new BacsPaymentStrategy();
        var account = new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs };
        var request = new MakePaymentRequest();

        // Act
        var result = strategy.CanProcess(account, request);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void GivenAccountWithFasterPayments_WhenUsingFasterPaymentsStrategy_ThenCanProcess()
    {
        // Arrange
        var strategy = new FasterPaymentsStrategy();
        var account = new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments };
        var request = new MakePaymentRequest();

        // Act
        var result = strategy.CanProcess(account, request);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void GivenAccountWithChaps_WhenUsingChapsPaymentStrategy_ThenCanProcess()
    {
        // Arrange
        var strategy = new ChapsPaymentStrategy();
        var account = new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps };
        var request = new MakePaymentRequest();

        // Act
        var result = strategy.CanProcess(account, request);

        // Assert
        Assert.True(result);
    }
}