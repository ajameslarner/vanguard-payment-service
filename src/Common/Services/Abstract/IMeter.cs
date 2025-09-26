namespace Common.Services.Abstract;

public interface IMeter
{
    public void RecordPaymentSuccess();
    public void RecordPaymentFailure();
}
