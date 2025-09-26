using System.Diagnostics.CodeAnalysis;
using Domain.Models;
using MediatR;

namespace Common.Queries;

[ExcludeFromCodeCoverage]
public class GetAccountQuery : IRequest<AccountModel>
{
    public GetAccountQuery(string accountNumber)
    {
        AccountNumber = accountNumber;
    }

    public string AccountNumber { get; }
}
