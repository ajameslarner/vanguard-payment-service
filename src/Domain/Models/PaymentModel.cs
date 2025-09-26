using System.Diagnostics.CodeAnalysis;
using Domain.Enums;

namespace Domain.Models;

[ExcludeFromCodeCoverage]
public class PaymentModel
{
    public Guid TransactionId { get; set; }
    public decimal Value { get; set; }
    public string DebtorAccountNumber { get; set; }
    public string CreditorAccountNumber { get; set; }
    public DateTimeOffset TransactionDate { get; set; }
    public PaymentScheme PaymentScheme { get; set; }
}
