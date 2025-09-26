using Common.Commands;
using Common.Queries;
using Common.Services.Abstract;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Common.Handlers;

public class AccountsHandler : IRequestHandler<GetAccountQuery, AccountModel>
{
    private readonly ILogger<AccountsHandler> _logger;
    private readonly IAccountService _accountService;

    public AccountsHandler(
        ILogger<AccountsHandler> logger,
        IAccountService accountService)
    {
        _logger = logger;
        _accountService = accountService;
    }

    public async Task<AccountModel> Handle(GetAccountQuery command, CancellationToken cancellationToken)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(command.AccountNumber, nameof(command.AccountNumber));

            return await _accountService.GetAccount(command.AccountNumber, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex?.InnerException?.Message ?? ex?.Message);

            throw;
        }
    }
}
