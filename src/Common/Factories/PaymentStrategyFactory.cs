using Common.Factories.Abstract;
using Common.Strategies.Abstract;
using Domain.Enums;

namespace Common.Factories;

public class PaymentStrategyFactory : IPaymentStrategyFactory
{
    private readonly Dictionary<PaymentScheme, IPaymentStrategy> _strategies;

    public PaymentStrategyFactory(IEnumerable<IPaymentStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.Scheme);
    }

    public IPaymentStrategy Resolve(PaymentScheme scheme)
    {
        if (!_strategies.TryGetValue(scheme, out var strategy))
            throw new NotSupportedException($"Unsupported scheme: {scheme}");

        return strategy;
    }
}
