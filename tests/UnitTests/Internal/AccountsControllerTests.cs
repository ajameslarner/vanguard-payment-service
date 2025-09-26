using Common.Queries;
using Domain.Models;
using Internal.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Vanguard.Tests.Internal;

public class AccountsControllerTests
{
    private readonly IMediator _mediator;
    private readonly AccountsController _controller;

    public AccountsControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _controller = new AccountsController(_mediator);
    }

    [Fact]
    public async Task GivenGetAccount_WhenAccountFound_ThenReturnsOk()
    {
        // Arrange
        var accountNumber = "123456";
        var accountModel = new AccountModel { AccountNumber = accountNumber };
        _mediator.Send(Arg.Any<GetAccountQuery>()).Returns(accountModel);

        // Act
        var result = await _controller.GetAccount(accountNumber);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(accountModel, okResult.Value);
    }

    [Fact]
    public async Task GivenGetAccount_WhenAccountNotFound_ThenReturnsProblem()
    {
        // Arrange
        var accountNumber = "123456";
        _mediator.Send(Arg.Any<GetAccountQuery>()).Returns((AccountModel)null!);

        // Act
        var result = await _controller.GetAccount(accountNumber);

        // Assert
        var problemResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains($"Cannot find an account matching: {accountNumber}", problemResult.Value?.ToString());
    }
}