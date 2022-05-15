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

        internal void UpdateAccount(MakePaymentRequest request, string dataStoreType, Account account, MakePaymentResult result)
        {
            try
            {
                if (result.Success)
                {
                    account.Balance -= request.Amount;

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

        internal MakePaymentResult GetPaymentResult(MakePaymentRequest request, Account account)
        {
            var result = new MakePaymentResult();
            
            switch (request.PaymentScheme)
            {
                case PaymentScheme.FasterPayments:
                    if (account == null || !account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments)
                        || account.Balance < request.Amount)
                    {
                        result.Success = false;
                        return result;
                    }
                    break;
                case PaymentScheme.Bacs:
                    if (account == null || !account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
                    {
                        result.Success = false;
                        return result;
                    }
                    break;
                case PaymentScheme.Chaps:
                    if (account == null || !account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps) ||
                        account.Status != AccountStatus.Live)
                    {
                        result.Success = false;
                        return result;
                    }
                    break;
            }
            return result;
        }

        
        public Account GetAccountBasedOnDataStoreType(MakePaymentRequest request, string dataStoreType)
        {
            if (dataStoreType == "Backup")
            {
                var backupAccountDataStore = new BackupAccountDataStore();
                return backupAccountDataStore.GetAccount(request.DebtorAccountNumber);
            }

            var accountDataStore = new AccountDataStore();
            return accountDataStore.GetAccount(request.DebtorAccountNumber);
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var dataStoreType= _configuration.GetValue<string>("DataStoreType");

            var account = GetAccountBasedOnDataStoreType(request, dataStoreType);

            MakePaymentResult result = GetPaymentResult(request, account);

            UpdateAccount(request, dataStoreType, account, result);

            return result;
        }
    }
}
