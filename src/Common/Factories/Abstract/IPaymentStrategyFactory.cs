using Common.Strategies.Abstract;
using Domain.Enums;

namespace Common.Factories.Abstract;

public interface IPaymentStrategyFactory
{
    public IPaymentStrategy Resolve(PaymentScheme scheme);
}
