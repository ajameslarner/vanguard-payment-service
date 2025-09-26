using System.Diagnostics.CodeAnalysis;
using Common.Strategies.Abstract;
using Domain.Enums;
using Domain.Requests;
using Infrastructure.Entities;

namespace Common.Strategies;

public class BacsPaymentStrategy : IPaymentStrategy
{
    [ExcludeFromCodeCoverage]
    public PaymentScheme Scheme => PaymentScheme.Bacs;

    public bool CanProcess(Account account, MakePaymentRequest request) 
        => account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs);
}
