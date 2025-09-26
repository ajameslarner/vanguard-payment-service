using Common.Commands;
using Common.Queries;
using Domain.Enums;
using Domain.Models;
using Domain.Requests;
using Domain.Responses;
using Internal.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Vanguard.Tests.Internal;

public class PaymentsControllerTests
{
    private readonly IMediator _mediator;
    private readonly PaymentsController _controller;

    public PaymentsControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _controller = new PaymentsController(_mediator);
    }

    [Fact]
    public async Task GivenMakePayment_WhenNullRequest_ThenReturnsBadRequest()
    {
        // Arrange
        MakePaymentRequest request = null!;

        // Act
        var result = await _controller.MakePayment(request);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains(nameof(MakePaymentRequest), badRequest.Value?.ToString());
    }

    [Fact]
    public async Task GivenMakePayment_WhenInvalidPaymentScheme_ThenReturnsBadRequest()
    {
        // Arrange
        var request = new MakePaymentRequest
        {
            PaymentScheme = (PaymentScheme)999
        };

        // Act
        var result = await _controller.MakePayment(request);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("not supported", badRequest.Value?.ToString());
    }

    [Fact]
    public async Task GivenMakePayment_WhenSuccessResponse_ThenReturnsOk()
    {
        // Arrange
        var request = new MakePaymentRequest { PaymentScheme = PaymentScheme.Bacs };
        var response = new MakePaymentResult { Success = true };
        _mediator.Send(Arg.Any<MakePaymentCommand>()).Returns(response);

        // Act
        var result = await _controller.MakePayment(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task GivenMakePayment_WhenFailureResponse_ThenReturnsProblem()
    {
        // Arrange
        var request = new MakePaymentRequest { PaymentScheme = PaymentScheme.Bacs };
        var response = new MakePaymentResult { Success = false, Details = "Insufficient funds" };
        _mediator.Send(Arg.Any<MakePaymentCommand>()).Returns(response);

        // Act
        var result = await _controller.MakePayment(request);

        // Assert
        var problemResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
        Assert.Contains("Insufficient funds", problemResult.Value?.ToString());
    }

    [Fact]
    public async Task GivenGetPayment_WhenPaymentFound_ThenReturnsOk()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var payment = new PaymentModel { TransactionId = transactionId };
        _mediator.Send(Arg.Any<GetPaymentQuery>()).Returns(payment);

        // Act
        var result = await _controller.GetPayment(transactionId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(payment, okResult.Value);
    }

    [Fact]
    public async Task GivenGetPayment_WhenPaymentNotFound_ThenReturnsNotFound()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        _mediator.Send(Arg.Any<GetPaymentQuery>()).Returns((PaymentModel)null!);

        // Act
        var result = await _controller.GetPayment(transactionId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains(transactionId.ToString(), notFoundResult.Value?.ToString());
    }
}