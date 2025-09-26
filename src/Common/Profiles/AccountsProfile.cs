using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Domain.Models;
using Infrastructure.Entities;

namespace Common.Profiles;

[ExcludeFromCodeCoverage]
internal class AccountProfile : Profile
{
    public AccountProfile()
    {
        CreateMap<Account, AccountModel>()
            .ReverseMap();
    }
}
