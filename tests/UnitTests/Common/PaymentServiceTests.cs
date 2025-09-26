using System.Linq.Expressions;
using AutoMapper;
using Common.Factories.Abstract;
using Common.Notifications;
using Common.Services;
using Common.Services.Abstract;
using Common.Strategies;
using Domain.Enums;
using Domain.Models;
using Domain.Requests;
using Infrastructure.Entities;
using Infrastructure.Repositories.Abstract;
using MediatR;
using NSubstitute;

namespace Vanguard.Tests.Common;

public class PaymentServiceTests
{
    private readonly IPaymentStrategyFactory _paymentStrategyFactory;
    private readonly IPaymentService _paymentService;
    private readonly IRepository<Payment> _paymentRepository;
    private readonly IRepository<Account> _accountRepository;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IMeter _meter;

    public PaymentServiceTests()
    {
        _paymentStrategyFactory = Substitute.For<IPaymentStrategyFactory>();
        _accountRepository = Substitute.For<IRepository<Account>>();
        _paymentRepository = Substitute.For<IRepository<Payment>>();
        _mediator = Substitute.For<IMediator>();
        _mapper = Substitute.For<IMapper>();
        _meter = Substitute.For<IMeter>();

        _paymentService = new PaymentService(_paymentStrategyFactory, _accountRepository, _paymentRepository, _mediator, _mapper, _meter);
    }

    [Fact]
    public async Task GivenMakePaymentAsync_WhenDebtorIsNull_ThenShouldThrowArgumentNullException()
    {
        // Arrange
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = null,
            CreditorAccountNumber = "Creditor123",
            Amount = 100.00m
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _paymentService.MakePaymentAsync(request);
        });
    }

    [Fact]
    public async Task GivenMakePaymentAsync_WhenCreditorIsNull_ThenShouldThrowArgumentNullException()
    {
        // Arrange
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "Debitor123",
            CreditorAccountNumber = null,
            Amount = 100.00m
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _paymentService.MakePaymentAsync(request);
        });
    }

    [Fact]
    public async Task GivenMakePaymentAsync_WhenDebitorIsNotFound_ThenShouldReturnFailure()
    {
        // Arrange
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "Debitor123",
            CreditorAccountNumber = "Creditor123",
            Amount = 100.00m
        };

        _accountRepository.FindOneAsync(Arg.Any<Expression<Func<Account, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Account>(null!));

        // Act
        var result = await _paymentService.MakePaymentAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal($"Failed to find the debtors account matching: {request.DebtorAccountNumber}", result.Details);
    }

    [Fact]
    public async Task GivenMakePaymentAsync_WhenDebitorBalanceIsTooLow_ThenShouldReturnFailure()
    {
        // Arrange
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "Debitor123",
            CreditorAccountNumber = "Creditor123",
            Amount = 100.00m
        };
        var debtorAccount = new Account
        {
            AccountNumber = "Debitor123",
            Balance = 50.00m
        };
        _accountRepository.FindOneAsync(Arg.Any<Expression<Func<Account, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(debtorAccount));
        
        // Act
        var result = await _paymentService.MakePaymentAsync(request);
        
        // Assert
        Assert.False(result.Success);
        Assert.Equal("Insufficient funds", result.Details);
    }

    [Fact]
    public async Task GivenMakePaymentAsync_WhenDebitorPaymentSchemeIsNotAllowedBacs_ThenShouldReturnFailure()
    {
        // Arrange
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "Debitor123",
            CreditorAccountNumber = "Creditor123",
            Amount = 100.00m,
            PaymentScheme = PaymentScheme.Bacs
        };
        var debtorAccount = new Account
        {
            AccountNumber = "Debitor123",
            Balance = 200.00m,
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments // Does not allow Bacs
        };
        _accountRepository.FindOneAsync(Arg.Any<Expression<Func<Account, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(debtorAccount));
        // Act
        var result = await _paymentService.MakePaymentAsync(request);
        
        // Assert
        Assert.False(result.Success);
        Assert.Equal($"Unable to process this payment under the scheme: {request.PaymentScheme}", result.Details);
    }

    [Fact]
    public async Task GivenMakePaymentAsync_WhenDebitorPaymentSchemeIsNotAllowedFasterPayments_ThenShouldReturnFailure()
    {
        // Arrange
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "Debitor123",
            CreditorAccountNumber = "Creditor123",
            Amount = 100.00m,
            PaymentScheme = PaymentScheme.FasterPayments
        };
        var debtorAccount = new Account
        {
            AccountNumber = "Debitor123",
            Balance = 200.00m,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs // Does not allow FasterPayments
        };
        _accountRepository.FindOneAsync(Arg.Any<Expression<Func<Account, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(debtorAccount));
        
        // Act
        var result = await _paymentService.MakePaymentAsync(request);
        
        // Assert
        Assert.False(result.Success);
        Assert.Equal($"Unable to process this payment under the scheme: {request.PaymentScheme}", result.Details);
    }

    [Fact]
    public async Task GivenMakePaymentAsync_WhenDebitorPaymentSchemeIsNotAllowedChaps_ThenShouldReturnFailure()
    {
        // Arrange
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "Debitor123",
            CreditorAccountNumber = "Creditor123",
            Amount = 100.00m,
            PaymentScheme = PaymentScheme.Chaps
        };
        var debtorAccount = new Account
        {
            AccountNumber = "Debitor123",
            Balance = 200.00m,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs // Does not allow Chaps
        };
        _accountRepository.FindOneAsync(Arg.Any<Expression<Func<Account, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(debtorAccount));
        
        // Act
        var result = await _paymentService.MakePaymentAsync(request);
        
        // Assert
        Assert.False(result.Success);
        Assert.Equal($"Unable to process this payment under the scheme: {request.PaymentScheme}", result.Details);
    }

    [Fact]
    public async Task GivenMakePaymentAsync_WhenDebitorPaymentSchemeIsAllowedBacs_ThenShouldReturnSuccess()
    {
        // Arrange
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "Debitor123",
            CreditorAccountNumber = "Creditor123",
            Amount = 100.00m,
            PaymentScheme = PaymentScheme.Bacs
        };
        var debtorAccount = new Account
        {
            AccountNumber = "Debitor123",
            Balance = 200.00m,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs // Allows Bacs
        };

        _accountRepository.FindOneAsync(Arg.Any<Expression<Func<Account, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var expr = callInfo.Arg<Expression<Func<Account, bool>>>();
                if (expr.Compile().Invoke(debtorAccount))
                    return Task.FromResult(debtorAccount);
                var creditorAccount = new Account { AccountNumber = "Creditor123" };
                if (expr.Compile().Invoke(creditorAccount))
                    return Task.FromResult(creditorAccount);
                return Task.FromResult<Account>(null!);
            });

        _paymentStrategyFactory.Resolve(Arg.Is<PaymentScheme>(x => x == PaymentScheme.Bacs))
            .Returns(new BacsPaymentStrategy());

        // Act
        var result = await _paymentService.MakePaymentAsync(request);
        
        // Assert
        Assert.True(result.Success);
        Assert.StartsWith($"Payment successfully processed with transaction id:", result.Details);
    }

    [Fact]
    public async Task GivenMakePaymentAsync_WhenDebitorPaymentSchemeIsAllowedFasterPayments_ThenShouldReturnSuccess()
    {
        // Arrange
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "Debitor123",
            CreditorAccountNumber = "Creditor123",
            Amount = 100.00m,
            PaymentScheme = PaymentScheme.FasterPayments
        };
        var debtorAccount = new Account
        {
            AccountNumber = "Debitor123",
            Balance = 200.00m,
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments // Allows FasterPayments
        };

        _accountRepository.FindOneAsync(Arg.Any<Expression<Func<Account, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var expr = callInfo.Arg<Expression<Func<Account, bool>>>();
                if (expr.Compile().Invoke(debtorAccount))
                    return Task.FromResult(debtorAccount);
                var creditorAccount = new Account { AccountNumber = "Creditor123" };
                if (expr.Compile().Invoke(creditorAccount))
                    return Task.FromResult(creditorAccount);
                return Task.FromResult<Account>(null!);
            });

        _paymentStrategyFactory.Resolve(Arg.Is<PaymentScheme>(x => x == PaymentScheme.FasterPayments))
            .Returns(new FasterPaymentsStrategy());

        // Act
        var result = await _paymentService.MakePaymentAsync(request);
        
        // Assert
        Assert.True(result.Success);
        Assert.StartsWith($"Payment successfully processed with transaction id:", result.Details);
    }

    [Fact]
    public async Task GivenMakePaymentAsync_WhenDebitorPaymentSchemeIsAllowedChaps_ThenShouldReturnSuccess()
    {
        // Arrange
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "Debitor123",
            CreditorAccountNumber = "Creditor123",
            Amount = 100.00m,
            PaymentScheme = PaymentScheme.Chaps
        };
        var debtorAccount = new Account
        {
            AccountNumber = "Debitor123",
            Balance = 200.00m,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps // Allows Chaps
        };
        _accountRepository.FindOneAsync(Arg.Any<Expression<Func<Account, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var expr = callInfo.Arg<Expression<Func<Account, bool>>>();
                if (expr.Compile().Invoke(debtorAccount))
                    return Task.FromResult(debtorAccount);
                var creditorAccount = new Account { AccountNumber = "Creditor123" };
                if (expr.Compile().Invoke(creditorAccount))
                    return Task.FromResult(creditorAccount);
                return Task.FromResult<Account>(null!);
            });

        _paymentStrategyFactory.Resolve(Arg.Is<PaymentScheme>(x => x == PaymentScheme.Chaps))
            .Returns(new ChapsPaymentStrategy());

        // Act
        var result = await _paymentService.MakePaymentAsync(request);
        
        // Assert
        Assert.True(result.Success);
        Assert.StartsWith($"Payment successfully processed with transaction id:", result.Details);
    }

    [Fact]
    public async Task GivenMakePaymentAsync_WhenCreditorAccountIsNotFound_ThenShouldReturnFailure()
    {
        // Arrange
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "Debitor123",
            CreditorAccountNumber = "Creditor123",
            Amount = 100.00m,
            PaymentScheme = PaymentScheme.FasterPayments
        };
        var debtorAccount = new Account
        {
            AccountNumber = "Debitor123",
            Balance = 200.00m,
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments // Allows FasterPayments
        };
        _accountRepository.FindOneAsync(Arg.Any<Expression<Func<Account, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var expr = callInfo.Arg<Expression<Func<Account, bool>>>();
                if (expr.Compile().Invoke(debtorAccount))
                    return Task.FromResult(debtorAccount);

                var creditorAccount = new Account { AccountNumber = "Creditor123" };
                if (expr.Compile().Invoke(creditorAccount))
                    return Task.FromResult<Account>(null!);

                return Task.FromResult<Account>(null!);
            });

        _paymentStrategyFactory.Resolve(Arg.Is<PaymentScheme>(x => x == PaymentScheme.FasterPayments))
            .Returns(new FasterPaymentsStrategy());

        // Act
        var result = await _paymentService.MakePaymentAsync(request);
        
        // Assert
        Assert.False(result.Success);
        Assert.Equal($"Failed to find the creditors account matching: {request.CreditorAccountNumber}", result.Details);
    }

    [Fact]
    public async Task GivenMakePaymentAsync_WhenValidPaymentRequestIsSent_ThenShouldTransferBalance()
    {
        // Arrange
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "Debitor123",
            CreditorAccountNumber = "Creditor123",
            Amount = 100.00m,
            PaymentScheme = PaymentScheme.FasterPayments
        };
        var debtorAccount = new Account
        {
            AccountNumber = "Debitor123",
            Balance = 200.00m, // Sufficient funds
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments // Allows FasterPayments
        };
        var creditorAccount = new Account
        {
            AccountNumber = "Creditor123",
            Balance = 50.00m
        };
        _accountRepository.FindOneAsync(Arg.Any<Expression<Func<Account, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var expr = callInfo.Arg<Expression<Func<Account, bool>>>();
                if (expr.Compile().Invoke(debtorAccount))
                    return Task.FromResult(debtorAccount);
                if (expr.Compile().Invoke(creditorAccount))
                    return Task.FromResult(creditorAccount);
                return Task.FromResult<Account>(null!);
            });
        _paymentStrategyFactory.Resolve(Arg.Is<PaymentScheme>(x => x == PaymentScheme.FasterPayments))
            .Returns(new FasterPaymentsStrategy());

        // Act
        var result = await _paymentService.MakePaymentAsync(request);
        
        // Assert
        Assert.True(result.Success);
        Assert.StartsWith($"Payment successfully processed with transaction id:", result.Details);
        Assert.Equal(100.00m, debtorAccount.Balance);
        Assert.Equal(150.00m, creditorAccount.Balance);
    }

    [Fact]
    public async Task GivenMakePaymentAsync_WhenValidPaymentRequestIsSent_ThenShouldPublishPaymentNotification()
    {
        // Arrange
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "Debitor123",
            CreditorAccountNumber = "Creditor123",
            Amount = 100.00m,
            PaymentScheme = PaymentScheme.FasterPayments
        };
        var debtorAccount = new Account
        {
            AccountNumber = "Debitor123",
            Balance = 200.00m, // Sufficient funds
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments // Allows FasterPayments
        };
        var creditorAccount = new Account
        {
            AccountNumber = "Creditor123",
            Balance = 50.00m
        };
        _accountRepository.FindOneAsync(Arg.Any<Expression<Func<Account, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var expr = callInfo.Arg<Expression<Func<Account, bool>>>();
                if (expr.Compile().Invoke(debtorAccount))
                    return Task.FromResult(debtorAccount);
                if (expr.Compile().Invoke(creditorAccount))
                    return Task.FromResult(creditorAccount);
                return Task.FromResult<Account>(null!);
            });
        _paymentStrategyFactory.Resolve(Arg.Is<PaymentScheme>(x => x == PaymentScheme.FasterPayments))
            .Returns(new FasterPaymentsStrategy());

        // Act
        var result = await _paymentService.MakePaymentAsync(request);
        
        // Assert
        await _mediator.Received(1).Publish(Arg.Is<PaymentNotification>(pn =>
            pn.Payment.DebtorAccountNumber == request.DebtorAccountNumber &&
            pn.Payment.CreditorAccountNumber == request.CreditorAccountNumber &&
            pn.Payment.Value == request.Amount &&
            pn.Payment.PaymentScheme == request.PaymentScheme), 
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GivenGetPaymentAsync_WhenPaymentExists_ThenShouldReturnPaymentModel()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var paymentEntity = new Payment
        {
            TransactionId = transactionId,
            DebtorAccountNumber = "Debitor123",
            CreditorAccountNumber = "Creditor123",
            Value = 100.00m,
            PaymentScheme = PaymentScheme.FasterPayments
        };
        var paymentModel = new PaymentModel
        {
            TransactionId = transactionId,
            DebtorAccountNumber = "Debitor123",
            CreditorAccountNumber = "Creditor123",
            Value = 100.00m,
            PaymentScheme = PaymentScheme.FasterPayments
        };
        _paymentRepository.FindOneAsync(Arg.Any<Expression<Func<Payment, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(paymentEntity));
        _mapper.Map<PaymentModel>(Arg.Any<Payment>())
            .Returns(paymentModel);

        // Act
        var result = await _paymentService.GetPayment(transactionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transactionId, result.TransactionId);
        Assert.Equal("Debitor123", result.DebtorAccountNumber);
        Assert.Equal("Creditor123", result.CreditorAccountNumber);
        Assert.Equal(100.00m, result.Value);
        Assert.Equal(PaymentScheme.FasterPayments, result.PaymentScheme);
    }

    [Fact]
    public async Task GivenGetPaymentAsync_WhenPaymentDoesNotExist_ThenShouldReturnNull()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        _paymentRepository.FindOneAsync(Arg.Any<Expression<Func<Payment, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Payment>(null!));
        // Act
        var result = await _paymentService.GetPayment(transactionId);
        // Assert
        Assert.Null(result);
    }
}