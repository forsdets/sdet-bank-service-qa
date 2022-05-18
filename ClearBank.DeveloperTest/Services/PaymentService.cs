using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using Microsoft.Extensions.Configuration;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ClearBank.DeveloperTest.Tests")]
namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;

        //Added an example of DI.
        public PaymentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        internal void UpdateAccount(MakePaymentRequest paymentRequest, string dataStoreType, Account account, MakePaymentResult paymentResult)
        {
            try
            {
                if (paymentResult.Success)
                {
                    account.Balance -= paymentRequest.Amount;

                    if (dataStoreType == "Backup")
                    {
                        var accountDataStore = new BackupAccountDataStore();
                        accountDataStore.UpdateAccount(account);
                    }
                    else
                    {
                        var accountDataStore = new AccountDataStore();
                        accountDataStore.UpdateAccount(account);
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("Payment is not success");
            }
        }

        internal MakePaymentResult GetPaymentResult(MakePaymentRequest paymentRequest, Account account)
        {
            var paymentResult = new MakePaymentResult();
            
            switch (paymentRequest.PaymentScheme)
            {
                case PaymentScheme.FasterPayments:
                    if (account == null || !account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments)
                        || account.Balance < paymentRequest.Amount)
                    {
                        paymentResult.Success = false;
                        return paymentResult;
                    }
                    break;
                case PaymentScheme.Bacs:
                    if (account == null || !account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
                    {
                        paymentResult.Success = false;
                        return paymentResult;
                    }
                    break;
                case PaymentScheme.Chaps:
                    if (account == null || !account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps) ||
                        account.Status != AccountStatus.Live)
                    {
                        paymentResult.Success = false;
                        return paymentResult;
                    }
                    break;
            }
            return paymentResult;
        }

        
        public Account GetDebtorAccountBasedOnDataStoreType(MakePaymentRequest paymentRequest, string dataStoreType)
        {
            if (dataStoreType == "Backup")
            {
                var backupAccountDataStore = new BackupAccountDataStore();
                return backupAccountDataStore.GetAccount(paymentRequest.DebtorAccountNumber);
            }

            var accountDataStore = new AccountDataStore();
            return accountDataStore.GetAccount(paymentRequest.DebtorAccountNumber);
        }

        public MakePaymentResult MakePayment(MakePaymentRequest paymentRequest)
        {
            var dataStoreType= _configuration.GetValue<string>("DataStoreType");
            var accountType = GetDebtorAccountBasedOnDataStoreType(paymentRequest, dataStoreType);
            MakePaymentResult paymentResult = GetPaymentResult(paymentRequest, accountType);
            UpdateAccount(paymentRequest, dataStoreType, accountType, paymentResult);

            return paymentResult;
        }
    }
}
