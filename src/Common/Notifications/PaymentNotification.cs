using Domain.Models;
using MediatR;

namespace Common.Notifications;

public class PaymentNotification : INotification
{
    public PaymentNotification(PaymentModel payment)
    {
        Payment = payment;
    }

    public PaymentModel Payment { get; set; }
}
