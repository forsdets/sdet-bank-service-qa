using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using Xunit;

namespace ClearBank.DeveloperTest.Tests
{
    public class ClearBankPaymentServiceTests
    {

        [Fact]
        public void TestAccountBasedOnDataStoreTypeNonBackup()
        {
            //Given mock
            var mockConfig = new Mock<IConfiguration>();
            //And set the payment service request
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Bacs
            };

            //When
            var paymentService = new PaymentService(mockConfig.Object);
            var accountNonBackup = paymentService.GetAccountTypeBasedOnDataStoreType(makePaymentRequest, "NonBackup");

            //Then
            accountNonBackup.Should().NotBeNull();
        }

        [Fact]
        public void TestGetAccountBasedOnDataStoreTypeBackup()
        {
            //Given mock
            var mockConfig = new Mock<IConfiguration>();
            //And set the payment service request
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Bacs
            };

            //When 
            var paymentService = new PaymentService(mockConfig.Object);
            var accountBackup = paymentService.GetAccountTypeBasedOnDataStoreType(makePaymentRequest, "Backup");

            //Then
            accountBackup.Should().NotBeNull();
        }


        [Fact]
        public void TestPaymentResultWithNullAccount()
        {
            //Given mock
            var configuration = new Mock<IConfiguration>();
            //And set the payment service request
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Bacs
            };
            Account account = null;

            //When
            var paymentService = new PaymentService(configuration.Object);
            var result = paymentService.GetPaymentResult(makePaymentRequest, account);

            //Then
            result.Success.Should().Be(false);
        }

        [Fact]
        public void TestPaymentResultForBacsPaymentSchemeWithAllowedPaymentSchemesBacs()
        {
            //Given mock
            var configuration = new Mock<IConfiguration>();
            //And set the payment service request
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Bacs
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs
            };

            //When
            var paymentService = new PaymentService(configuration.Object);
            var result = paymentService.GetPaymentResult(makePaymentRequest, account);

            //Then
            result.Success.Should().Be(true);
        }

        [Fact]
        public void TestPaymentResultForBacsPaymentSchemeWithAllowedPaymentSchemesChaps()
        {
            //Given mock
            var configuration = new Mock<IConfiguration>();
            //And set the payment service request
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Bacs
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps
            };

            //When
            var paymentService = new PaymentService(configuration.Object);
            var result = paymentService.GetPaymentResult(makePaymentRequest, account);

            //Then
            result.Success.Should().Be(false);
        }

        [Fact]
        public void TestPaymentResultForBacsPaymentSchemeWithAllowedPaymentSchemesFasterPayments()
        {
            //Given mock
            var configuration = new Mock<IConfiguration>();
            //And set the payment service request
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Bacs
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments
            };

            //When
            var paymentService = new PaymentService(configuration.Object);
            var result = paymentService.GetPaymentResult(makePaymentRequest, account);

            //Then
            result.Success.Should().Be(false);
        }

        [Fact]
        public void TestPaymentResultForFasterPaymentsSchemeAndAccountBalancegreaterThanRequest()
        {
            //Given mock
            var configuration = new Mock<IConfiguration>();
            //And set the payment service request
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.FasterPayments,
                Amount = 5
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Balance = 10
            };

            //When
            var paymentService = new PaymentService(configuration.Object);
            var result = paymentService.GetPaymentResult(makePaymentRequest, account);

            //Then
            result.Success.Should().Be(true);
        }

        [Fact]
        public void TestPaymentResultForFasterPaymentsSchemAndAccountBalanceLessThanRequest()
        {
            //Given mock
            var configuration = new Mock<IConfiguration>();
            //And set the payment service request
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.FasterPayments,
                Amount = 10
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Balance = 5
            };

            //When
            var paymentService = new PaymentService(configuration.Object);
            var result = paymentService.GetPaymentResult(makePaymentRequest, account);

            //Then
            result.Success.Should().Be(false);
        }
        [Fact]
        public void TestPaymentResultForFasterPaymentsSchemeAsNonFasterPaymentsAndAccountBalanceGreaterThanRequest()
        {
            //Given mock
            var configuration = new Mock<IConfiguration>();
            //And set the payment service request
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.FasterPayments,
                Amount = 20
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
                Balance = 200
            };

            //When
            var paymentService = new PaymentService(configuration.Object);
            var result = paymentService.GetPaymentResult(makePaymentRequest, account);

            //Then
            result.Success.Should().Be(false);
        }

        [Fact]
        public void TestPaymentResultForChapsAccountWithChapsPaymentSchemeAndAccountStatusLive()
        {
            //Given mock
            var configuration = new Mock<IConfiguration>();
            //And set the payment service request
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Chaps,
                Amount = 20
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = AccountStatus.Live
            };

            //When
            var paymentService = new PaymentService(configuration.Object);
            var result = paymentService.GetPaymentResult(makePaymentRequest, account);

            //Then
            result.Success.Should().Be(true);
        }

        [Fact]
        public void TestPaymentResultForChapsAccountWithNonChapsPaymentSchemeAndAccountStatusLive()
        {
            //Given
            var configuration = new Mock<IConfiguration>();
            //And set the payment service request
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Chaps,
                Amount = 20
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
                Status = AccountStatus.Live
            };

            //When
            var paymentService = new PaymentService(configuration.Object);
            var result = paymentService.GetPaymentResult(makePaymentRequest, account);

            //Then
            result.Success.Should().Be(false);
        }

        [Fact]
        public void TestPaymentResultForChapsAccountWithChapsPaymentSchemeAndAccountStatusNonLive()
        {
            //Given mock
            var configuration = new Mock<IConfiguration>();
            //And set the payment service request
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Chaps,
                Amount = 20
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = AccountStatus.Disabled
            };

            //When
            var paymentService = new PaymentService(configuration.Object);
            var result = paymentService.GetPaymentResult(makePaymentRequest, account);

            //Then
            result.Success.Should().Be(false);
        }

        [Fact]
        public void TestPaymentResultForChapsAccountWithChapsPaymentSchemeAndAccountStatuInboundPaymentsOnly()
        {
            //Given mock
            var configuration = new Mock<IConfiguration>();
            //And set the payment service request
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Chaps,
                Amount = 20
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = AccountStatus.InboundPaymentsOnly
            };

            //When
            var paymentService = new PaymentService(configuration.Object);
            var result = paymentService.GetPaymentResult(makePaymentRequest, account);

            //Then
            result.Success.Should().Be(false);
        }

        [Fact]
        public void TestUpdateAccountWithPaymentResultFailure()
        {
            //Given mock
            var configuration = new Mock<IConfiguration>();
            //And set the payment service request
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Chaps,
                Amount = 20                
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = AccountStatus.Disabled
            };
            var makePaymentResult = new MakePaymentResult()
            {
                Success = false
            };

            //When
            var paymentService = new PaymentService(configuration.Object);
            Action act = () => paymentService.UpdateAccount(makePaymentRequest, "Backup", account, null);

            //Then
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Payment is not success");
        }

        [Fact]
        public void TestUpdateAccountWithPaymentResultSuccess()
        {
            //Given mock
            var configuration = new Mock<IConfiguration>();
            //And set the payment service request
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Chaps,
                Amount = 20
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
            };
            var makePaymentResult = new MakePaymentResult()
            {
                Success = true
            };

            //When
            var paymentService = new PaymentService(configuration.Object);
            Action act = () => paymentService.UpdateAccount(makePaymentRequest, "Backup", account, makePaymentResult);

            //Then
            act.Should().NotThrow<InvalidOperationException>();
        }

        [Fact]
        public void TestMakePaymentRequest()
        {
            //Given mock
            var configuration = new Mock<IConfiguration>();
            var configurationSection = new Mock<IConfigurationSection>();
            configurationSection.Setup(a => a.Value).Returns("Backup");
            configuration.Setup(a => a.GetSection("DataStoreType")).Returns(configurationSection.Object);
            //And set the payment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Chaps,
                Amount = 20
            };

            //When
            var paymentService = new PaymentService(configuration.Object);
            var result = paymentService.MakePayment(makePaymentRequest);

            //Then
            result.Success.Should().Be(false);
        }
    }
}
