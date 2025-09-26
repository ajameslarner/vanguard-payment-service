using System.Diagnostics.CodeAnalysis;
using Domain.Enums;

namespace Domain.Models;

[ExcludeFromCodeCoverage]
public class AccountModel
{
    public string AccountNumber { get; set; }
    public decimal Balance { get; set; }
    public AccountStatus Status { get; set; }
    public AllowedPaymentSchemes AllowedPaymentSchemes { get; set; }
}
