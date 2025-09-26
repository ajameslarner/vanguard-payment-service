using AutoMapper;
using Common.Commands;
using Common.Notifications;
using Common.Queries;
using Common.Services.Abstract;
using Domain.Models;
using Domain.Responses;
using Infrastructure.Entities;
using Infrastructure.Repositories.Abstract;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Common.Handlers;

public class PaymentsHandler : 
    IRequestHandler<MakePaymentCommand, MakePaymentResult>, 
    IRequestHandler<GetPaymentQuery, PaymentModel>,
    INotificationHandler<PaymentNotification>
{
    private readonly ILogger<PaymentsHandler> _logger;
    private readonly IPaymentService _paymentService;
    private readonly IRepository<Payment> _paymentsRepository;
    private readonly IMapper _mapper;

    public PaymentsHandler(
        ILogger<PaymentsHandler> logger, 
        IPaymentService paymentService,
        IRepository<Payment> paymentsRepository,
        IMapper mapper)
    {
        _logger = logger;
        _paymentService = paymentService;
        _paymentsRepository = paymentsRepository;
        _mapper = mapper;
    }

    public async Task<MakePaymentResult> Handle(MakePaymentCommand command, CancellationToken cancellationToken)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(command.Request, nameof(command.Request));

            return await _paymentService.MakePaymentAsync(command.Request, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex?.InnerException?.Message ?? ex?.Message);

            throw;
        }
    }

    public async Task<PaymentModel> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request.TransactionId, nameof(request.TransactionId));

            return await _paymentService.GetPayment(request.TransactionId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex?.InnerException?.Message ?? ex?.Message);

            throw;
        }
    }

    public async Task Handle(PaymentNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            var payment = _mapper.Map<Payment>(notification.Payment);
            await _paymentsRepository.InsertOneAsync(payment, true, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex?.InnerException?.Message ?? ex?.Message);

            throw;
        }
    }
}
