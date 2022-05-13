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
        public void TestGetAccountBasedOnDataStoreTypeBackup()
        {
            //Given mock
            var mockConfig = new Mock<IConfiguration>();
            //And set the payment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Bacs
            };

            //When 
            var paymentService = new PaymentService(mockConfig.Object);
            var accountBackup = paymentService.GetAccountBasedOnDataStoreType(makePaymentRequest, "Backup");

            //Then
            accountBackup.Should().NotBeNull();
        }

        [Fact]
        public void TestAccountBasedOnDataStoreTypeNonBackup()
        {
            //Given
            var configuration = new Mock<IConfiguration>();
            //And set the payment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Bacs
            };

            //When
            var paymentService = new PaymentService(configuration.Object);
            var accountNonBackup = paymentService.GetAccountBasedOnDataStoreType(makePaymentRequest, "NonBackup");

            //Then
            accountNonBackup.Should().NotBeNull();
        }

        [Fact]
        public void TestMakePaymentResultAccountNull()
        {
            //Given mock
            var configuration = new Mock<IConfiguration>();
            //And set the payment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Bacs
            };
            Account account = null;

            //When
            var paymentService = new PaymentService(configuration.Object);
            var result = paymentService.GetMakePaymentResult(makePaymentRequest, account);

            //Then
            result.Success.Should().Be(false);
        }

        [Fact]
        public void TestMakePaymentResultBacs()
        {
            //Given mock
            var configuration = new Mock<IConfiguration>();
            //And set the payment service
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
            var result = paymentService.GetMakePaymentResult(makePaymentRequest, account);

            //Then
            result.Success.Should().Be(true);
        }

        [Fact]
        public void TestMakePaymentResultBacsAccountNonBacs()
        {
            //Given mock
            var configuration = new Mock<IConfiguration>();
            //And set the payment service
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
            var result = paymentService.GetMakePaymentResult(makePaymentRequest, account);

            //Then
            result.Success.Should().Be(false);
        }

        [Fact]
        public void TestMakePaymentResultFasterPaymentsSchemeAndAccountBalancegreaterThanRequest()
        {
            //Given mock
            var configuration = new Mock<IConfiguration>();
            //And set the payment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.FasterPayments,
                Amount = 10
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Balance = 100
            };

            //When
            var paymentService = new PaymentService(configuration.Object);
            var result = paymentService.GetMakePaymentResult(makePaymentRequest, account);

            //Then
            result.Success.Should().Be(true);
        }

        [Fact]
        public void TestMakePaymentResultFasterPaymentsSchemAndAccountBalanceLessThanRequest()
        {
            //Given mock
            var configuration = new Mock<IConfiguration>();
            //And set the payment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.FasterPayments,
                Amount = 100
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Balance = 10
            };

            //When
            var paymentService = new PaymentService(configuration.Object);
            var result = paymentService.GetMakePaymentResult(makePaymentRequest, account);

            //Then
            result.Success.Should().Be(false);
        }
        [Fact]
        public void TestMakePaymentResultForFasterPaymentsSchemeAsNonFasterPaymentsAndAccountBalanceGreaterThanRequest()
        {
            //Given mock
            var configuration = new Mock<IConfiguration>();
            //And set the payment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.FasterPayments,
                Amount = 10
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
                Balance = 100
            };

            //When
            var paymentService = new PaymentService(configuration.Object);
            var result = paymentService.GetMakePaymentResult(makePaymentRequest, account);

            //Then
            result.Success.Should().Be(false);
        }

        [Fact]
        public void TestMakePaymentResultChapsAccountChapsAndAccountStatusLive()
        {
            //Given mock
            var configuration = new Mock<IConfiguration>();
            //And set the payment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Chaps,
                Amount = 10
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = AccountStatus.Live
            };

            //When
            var paymentService = new PaymentService(configuration.Object);
            var result = paymentService.GetMakePaymentResult(makePaymentRequest, account);

            //Then
            result.Success.Should().Be(true);
        }

        [Fact]
        public void TestMakePaymentResultChapsAccountNonChapsAndAccountStatusLive()
        {
            //Given
            var configuration = new Mock<IConfiguration>();
            //And set the payment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Chaps,
                Amount = 10
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
                Status = AccountStatus.Live
            };

            //When
            var paymentService = new PaymentService(configuration.Object);
            var result = paymentService.GetMakePaymentResult(makePaymentRequest, account);

            //Then
            result.Success.Should().Be(false);
        }

        [Fact]
        public void TestMakePaymentResultChapsAccountChapsAndAccountStatusNonLive()
        {
            //Given mock
            var configuration = new Mock<IConfiguration>();
            //And set the payment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Chaps,
                Amount = 10
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = AccountStatus.Disabled
            };

            //When
            var paymentService = new PaymentService(configuration.Object);
            var result = paymentService.GetMakePaymentResult(makePaymentRequest, account);

            //Then
            result.Success.Should().Be(false);
        }

        [Fact]
        public void TestUpdateAccountMakePaymentResultFailure()
        {
            //Given mock
            var configuration = new Mock<IConfiguration>();
            //And set the payment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Chaps,
                Amount = 10                
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
            var ps = new PaymentService(configuration.Object);
            Action act = () => ps.UpdateAccount(makePaymentRequest, "Backup", account, null);

            //Then
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Payment is not success");
        }

        [Fact]
        public void TestUpdateAccountMakePaymentResultSuccess()
        {
            //Given mock
            var configuration = new Mock<IConfiguration>();
            //And set the payment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Chaps,
                Amount = 10
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = AccountStatus.Disabled
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
                Amount = 10
            };

            //When
            var paymentService = new PaymentService(configuration.Object);
            var result = paymentService.MakePayment(makePaymentRequest);

            //Then
            result.Success.Should().Be(false);
        }
    }
}
