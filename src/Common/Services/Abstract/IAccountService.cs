using Domain.Models;

namespace Common.Services.Abstract;

public interface IAccountService
{
    public Task<AccountModel> GetAccount(string accountNumber, CancellationToken cancellationToken = default);
}
