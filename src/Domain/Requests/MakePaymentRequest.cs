using System.Diagnostics.CodeAnalysis;
using Domain.Enums;

namespace Domain.Requests;

[ExcludeFromCodeCoverage]
public class MakePaymentRequest
{
    public string CreditorAccountNumber { get; set; }

    public string DebtorAccountNumber { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public PaymentScheme PaymentScheme { get; set; }
}
