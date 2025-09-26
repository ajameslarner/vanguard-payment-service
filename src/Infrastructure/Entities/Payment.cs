using System.Diagnostics.CodeAnalysis;
using Domain.Enums;
using Infrastructure.Entities.Base;

namespace Infrastructure.Entities;

[ExcludeFromCodeCoverage]
public class Payment : TEntity
{
    public Guid TransactionId { get; set; }
    public decimal Value { get; set; }
    public string DebtorAccountNumber { get; set; }
    public string CreditorAccountNumber { get; set; }
    public DateTimeOffset TransactionDate { get; set; }
    public PaymentScheme PaymentScheme { get; set; }
}
