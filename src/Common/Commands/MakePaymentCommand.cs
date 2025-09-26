using System.Diagnostics.CodeAnalysis;
using Domain.Requests;
using Domain.Responses;
using MediatR;

namespace Common.Commands;

[ExcludeFromCodeCoverage]
public class MakePaymentCommand : IRequest<MakePaymentResult>
{
    public MakePaymentCommand(MakePaymentRequest request)
    {
        Request = request;
    }

    public MakePaymentRequest Request { get; set; }
}
