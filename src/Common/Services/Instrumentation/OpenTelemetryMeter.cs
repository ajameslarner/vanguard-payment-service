using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using Common.Services.Abstract;
using Microsoft.Extensions.Configuration;

namespace Common.Services.Instrumentation;

[ExcludeFromCodeCoverage]
public class OpenTelemetryMeter : IMeter
{
    private Counter<int> SuccessfullPayments { get; }
    private Counter<int> FailedPayments { get; }


    public OpenTelemetryMeter(IConfiguration configuration, IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(configuration["ServiceName"]);

        SuccessfullPayments = meter.CreateCounter<int>("payment.success", "Success", "Number of successful payments processed");
        FailedPayments = meter.CreateCounter<int>("payment.failure", "Failure", "Number of failed payments processed");
    }

    public void RecordPaymentSuccess() => SuccessfullPayments.Add(1);
    public void RecordPaymentFailure() => FailedPayments.Add(1);
}