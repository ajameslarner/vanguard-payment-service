using System.Diagnostics.CodeAnalysis;
using Domain.Enums;
using Infrastructure.Entities.Base;

namespace Infrastructure.Entities;

[ExcludeFromCodeCoverage]
public class Account : TEntity
{
    public string AccountNumber { get; set; }
    public decimal Balance { get; set; }
    public AccountStatus Status { get; set; }
    public AllowedPaymentSchemes AllowedPaymentSchemes { get; set; }
}
