using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public interface IPaymentService
    {
        MakePaymentResult MakePayment(MakePaymentRequest request);
        Account GetAccountBasedOnDataStoreType(MakePaymentRequest request, string dataStoreType);
    }
}
