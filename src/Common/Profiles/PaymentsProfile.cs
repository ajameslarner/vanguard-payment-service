using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Domain.Models;
using Infrastructure.Entities;

namespace Common.Profiles;

[ExcludeFromCodeCoverage]
internal class PaymentsProfile : Profile
{
    public PaymentsProfile()
    {
        CreateMap<Payment, PaymentModel>()
            .ReverseMap();
    }
}
