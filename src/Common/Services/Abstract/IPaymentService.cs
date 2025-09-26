using Domain.Models;
using Domain.Requests;
using Domain.Responses;

namespace Common.Services.Abstract;

public interface IPaymentService
{
    public Task<PaymentModel> GetPayment(Guid transactionId, CancellationToken cancellationToken = default);
    public Task<MakePaymentResult> MakePaymentAsync(MakePaymentRequest request, CancellationToken cancellationToken = default);
    public MakePaymentResult MakePayment(MakePaymentRequest request);
}
