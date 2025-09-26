using AutoMapper;
using Common.Services.Abstract;
using Domain.Models;
using Infrastructure.Entities;
using Infrastructure.Repositories.Abstract;

namespace Common.Services;

public class AccountService : IAccountService
{
    private readonly IRepository<Account> _repository;
    private readonly IMapper _mapper;

    public AccountService(
        IRepository<Account> accountsRepository,
        IMapper mapper)
    {
        _repository = accountsRepository;
        _mapper = mapper;
    }

    public async Task<AccountModel> GetAccount(string accountNumber, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountNumber, nameof(accountNumber));

        var account = await _repository.FindOneAsync(a => a.AccountNumber == accountNumber, cancellationToken);

        if (account == null)
            return default;

        return _mapper.Map<AccountModel>(account);
    }
}
