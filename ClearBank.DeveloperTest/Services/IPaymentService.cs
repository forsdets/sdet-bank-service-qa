using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public interface IPaymentService
    {
        MakePaymentResult MakePayment(MakePaymentRequest request);
        Account GetAccountTypeBasedOnDataStoreType(MakePaymentRequest request, string dataStoreType);
    }
}
