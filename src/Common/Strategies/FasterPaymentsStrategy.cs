using System.Diagnostics.CodeAnalysis;
using Common.Strategies.Abstract;
using Domain.Enums;
using Domain.Requests;
using Infrastructure.Entities;

namespace Common.Strategies;

public class FasterPaymentsStrategy : IPaymentStrategy
{
    [ExcludeFromCodeCoverage]
    public PaymentScheme Scheme => PaymentScheme.FasterPayments;

    public bool CanProcess(Account account, MakePaymentRequest request) 
        => account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments);
}