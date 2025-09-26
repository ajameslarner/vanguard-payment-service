using System.Diagnostics.CodeAnalysis;
using Domain.Models;
using MediatR;

namespace Common.Queries;

[ExcludeFromCodeCoverage]
public class GetPaymentQuery : IRequest<PaymentModel>
{
    public GetPaymentQuery(Guid transactionId)
    {
        TransactionId = transactionId;
    }

    public Guid TransactionId { get; }
}
