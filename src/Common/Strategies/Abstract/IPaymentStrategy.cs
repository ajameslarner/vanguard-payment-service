using Domain.Enums;
using Domain.Requests;
using Infrastructure.Entities;

namespace Common.Strategies.Abstract;

public interface IPaymentStrategy
{
    public PaymentScheme Scheme { get; }
    public bool CanProcess(Account account, MakePaymentRequest request);
}