using System.Linq.Expressions;
using AutoMapper;
using Common.Services;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Entities;
using Infrastructure.Repositories.Abstract;
using NSubstitute;

namespace Vanguard.Tests.Common;

public class AccountServiceTests
{
    private readonly IRepository<Account> _accountRepository;
    private readonly IMapper _mapper;
    private readonly AccountService _accountService;

    public AccountServiceTests()
    {
        _accountRepository = Substitute.For<IRepository<Account>>();
        _mapper = Substitute.For<IMapper>();
        _accountService = new AccountService(_accountRepository, _mapper);
    }

    [Fact]
    public async Task GiveGetAccount_WhenAccountNumberIsNull_ThenThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _accountService.GetAccount(null));
    }

    [Fact]
    public async Task GiveGetAccount_WhenAccountNumberWhiteSpace_ThenThrowsArgumentException()
    {
        // Arrange, Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _accountService.GetAccount(""));
    }

    [Fact]
    public async Task GivenGetAccount_WhenAccountIsNotFound_ThenReturnsNull()
    {
        // Arrange
        var accountNumber = "123";
        _accountRepository.FindOneAsync(Arg.Any<Expression<Func<Account, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Account>(null!));

        // Act
        var result = await _accountService.GetAccount(accountNumber);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GivenGetAccount_WhenAccountIsFound_ThenReturnsMappedAccountModel()
    {
        // Arrange
        var accountNumber = "123";
        var account = new Account { AccountNumber = accountNumber };
        var accountModel = new AccountModel { AccountNumber = accountNumber };

        _accountRepository.FindOneAsync(Arg.Any<Expression<Func<Account, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(account);

        _mapper.Map<AccountModel>(account)
            .Returns(accountModel);

        // Act
        var result = await _accountService.GetAccount(accountNumber);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(accountNumber, result.AccountNumber);
    }
}