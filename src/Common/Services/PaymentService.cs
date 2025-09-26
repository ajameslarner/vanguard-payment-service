using AutoMapper;
using Common.Factories.Abstract;
using Common.Notifications;
using Common.Services.Abstract;
using Domain.Enums;
using Domain.Models;
using Domain.Requests;
using Domain.Responses;
using Infrastructure.Entities;
using Infrastructure.Repositories.Abstract;
using MediatR;

namespace Common.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentStrategyFactory _strategyFactory;
    private readonly IRepository<Account> _accountRepository;
    private readonly IRepository<Payment> _paymentRepository;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IMeter _meter;

    public PaymentService(
        IPaymentStrategyFactory strategyFactory,
        IRepository<Account> accountRepository,
        IRepository<Payment> paymentRepository,
        IMediator mediator,
        IMapper mapper,
        IMeter meter)
    {
        _strategyFactory = strategyFactory;
        _accountRepository = accountRepository;
        _paymentRepository = paymentRepository;
        _mediator = mediator;
        _mapper = mapper;
        _meter = meter;
    }

    public async Task<PaymentModel> GetPayment(Guid transactionId, CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.FindOneAsync(x => x.TransactionId == transactionId, cancellationToken);

        if (payment is null)
            return default;

        return _mapper.Map<PaymentModel>(payment);
    }

    public async Task<MakePaymentResult> MakePaymentAsync(MakePaymentRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.DebtorAccountNumber, nameof(request.DebtorAccountNumber));
        ArgumentException.ThrowIfNullOrWhiteSpace(request.CreditorAccountNumber, nameof(request.CreditorAccountNumber));

        var debtorAccount = await _accountRepository.FindOneAsync(a => a.AccountNumber == request.DebtorAccountNumber, cancellationToken);

        if (debtorAccount is null)
            return Failure($"Failed to find the debtors account matching: {request.DebtorAccountNumber}");

        if (debtorAccount.Status != AccountStatus.Live)
            return Failure("Debtors account is not live");

        if (debtorAccount.Balance < request.Amount)
            return Failure("Insufficient funds");

        var strategy = _strategyFactory.Resolve(request.PaymentScheme);
        var canProcess = strategy.CanProcess(debtorAccount, request);

        if (!canProcess)
            return Failure($"Unable to process this payment under the scheme: {request.PaymentScheme}");

        var creditorAccount = await _accountRepository.FindOneAsync(a => a.AccountNumber == request.CreditorAccountNumber, cancellationToken);

        if (creditorAccount is null)
            return Failure($"Failed to find the creditors account matching: {request.CreditorAccountNumber}");

        if (creditorAccount.Status != AccountStatus.Live)
            return Failure("Creditors account is not live");

        debtorAccount.Balance -= request.Amount;
        creditorAccount.Balance += request.Amount;

        await _accountRepository.ReplaceManyAsync([creditorAccount, debtorAccount], cancellationToken: cancellationToken);
        
        var paymentModel = new PaymentModel
        {
            TransactionId = Guid.NewGuid(),
            CreditorAccountNumber = request.CreditorAccountNumber,
            DebtorAccountNumber = request.DebtorAccountNumber,
            PaymentScheme = request.PaymentScheme,
            TransactionDate = DateTimeOffset.UtcNow,
            Value = request.Amount
        };

        await _mediator.Publish(new PaymentNotification(paymentModel), cancellationToken);

        return Success($"Payment successfully processed with transaction id: {paymentModel.TransactionId}");
    }

    public MakePaymentResult MakePayment(MakePaymentRequest request)
        => MakePaymentAsync(request).GetAwaiter().GetResult();

    private MakePaymentResult Success(string details)
    {
        _meter.RecordPaymentSuccess();
        return new() { Success = true, Details = details };
    }

    private MakePaymentResult Failure(string details)
    {
        _meter.RecordPaymentFailure();
        return new() { Success = false, Details = details };
    }
}
