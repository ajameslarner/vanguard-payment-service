using System.Diagnostics.CodeAnalysis;

namespace Domain.Responses;

[ExcludeFromCodeCoverage]
public class MakePaymentResult
{
    public bool Success { get; set; }
    public string Details { get; set; }
}
