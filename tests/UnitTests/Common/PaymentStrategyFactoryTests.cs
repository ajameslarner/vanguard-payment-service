using Common.Factories;
using Common.Strategies.Abstract;
using Domain.Enums;
using NSubstitute;

namespace Vanguard.Tests.Common;

public class PaymentStrategyFactoryTests
{
    [Fact]
    public void GivenStrategies_WhenResolveWithSupportedScheme_ThenReturnsCorrectStrategy()
    {
        // Arrange
        var bacsStrategy = Substitute.For<IPaymentStrategy>();
        bacsStrategy.Scheme.Returns(PaymentScheme.Bacs);

        var chapsStrategy = Substitute.For<IPaymentStrategy>();
        chapsStrategy.Scheme.Returns(PaymentScheme.Chaps);

        var factory = new PaymentStrategyFactory([bacsStrategy, chapsStrategy]);

        // Act
        var result = factory.Resolve(PaymentScheme.Bacs);

        // Assert
        Assert.Equal(bacsStrategy, result);
    }

    [Fact]
    public void GivenStrategies_WhenResolveWithUnsupportedScheme_ThenThrowsNotSupportedException()
    {
        // Arrange
        var bacsStrategy = Substitute.For<IPaymentStrategy>();
        bacsStrategy.Scheme.Returns(PaymentScheme.Bacs);

        var factory = new PaymentStrategyFactory([bacsStrategy]);

        // Act & Assert
        Assert.Throws<NotSupportedException>(() => factory.Resolve(PaymentScheme.FasterPayments));
    }

    [Fact]
    public void GivenEmptyStrategies_WhenResolve_ThenThrowsNotSupportedException()
    {
        // Arrange
        var factory = new PaymentStrategyFactory([]);

        // Act & Assert
        Assert.Throws<NotSupportedException>(() => factory.Resolve(PaymentScheme.Bacs));
    }
}