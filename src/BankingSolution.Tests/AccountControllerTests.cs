using BankingSolution.API.Controllers;
using BankingSolution.API.DTO;
using BankingSolution.Application.Interfaces;
using BankingSolution.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BankingSolution.Tests
{
    [TestFixture]
    public class AccountControllerTests
    {
        private Mock<IAccountService> _accountServiceMock;
        private AccountController _accountController;
        private string _accountNumber;
        private Currency _currency;
        private decimal _balance;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _accountNumber = "1200457721415111";
            _currency = Currency.USD;
            _balance = 100m;
        }

        [SetUp]
        public void Setup()
        {
            _accountServiceMock = new Mock<IAccountService>();
            _accountController = new AccountController(_accountServiceMock.Object,
                new Mock<ILogger<AccountController>>().Object);
        }

        #region GetAllAccounts Tests

        [Test]
        public async Task GetAllAccounts_ReturnsResponseStatus200WithAccountsTest()
        {
            // Arrange
            var secondAccountNumber = "8900787854778888";
            var expectedAccounts = new List<Account>
            {
                new Account(_accountNumber, _balance, _currency),
                new Account(secondAccountNumber, _balance, _currency),
            };
            _accountServiceMock.Setup(srv => srv.GetAllAccountsAsync())
                                  .ReturnsAsync(expectedAccounts);

            // Act
            var actualResponse = await _accountController.GetAllAccounts() as ActionResult;

            // Assert
            Assert.IsNotNull(actualResponse);
            Assert.IsInstanceOf<OkObjectResult>(actualResponse);
            var actualTypeResponse = actualResponse as OkObjectResult;
            Assert.That(actualTypeResponse.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            var actualBodyResponse = actualTypeResponse.Value as List<Account>;
            Assert.That(expectedAccounts.Count, Is.EqualTo(actualBodyResponse.Count));
            for (int i = 0; i < expectedAccounts.Count; i++)
            {
                AssertAccounts(expectedAccounts[i], actualBodyResponse[i]);
            }
            _accountServiceMock.Verify(srv => srv.GetAllAccountsAsync(), Times.Once);
        }

        #endregion
        
        #region GetAccountDetails Tests

        [Test]
        public async Task GetAccountDetails_ExistingAccountNumber_ReturnsResponseStatus200WithAccountTest()
        {
            // Arrange
            var expectedAccount = new Account(_accountNumber, _balance, _currency);
            _accountServiceMock.Setup(srv => srv.GetAccountDetailsAsync(_accountNumber))
                                  .ReturnsAsync(expectedAccount);

            // Act
            var actualResponse = await _accountController.GetAccountDetails(_accountNumber) as ActionResult;

            // Assert
            Assert.IsNotNull(actualResponse);
            Assert.IsInstanceOf<OkObjectResult>(actualResponse);
            var actualTypeResponse = actualResponse as OkObjectResult;
            Assert.That(actualTypeResponse.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            var actualBodyResponse = actualTypeResponse.Value as Account;
            AssertAccounts(expectedAccount, actualBodyResponse);
            _accountServiceMock.Verify(srv => srv.GetAccountDetailsAsync(It.IsAny<string>()), Times.Once);
        }
        
        [Test]
        public async Task GetAccountDetails_NonExistingAccountNumber_ReturnsNotFoundResultTest()
        {
            // Arrange
            _accountServiceMock.Setup(srv => srv.GetAccountDetailsAsync(_accountNumber))
                                  .ReturnsAsync((Account)null);

            // Act
            var actualResponse = await _accountController.GetAccountDetails(_accountNumber);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actualResponse);
            _accountServiceMock.Verify(srv => srv.GetAccountDetailsAsync(It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region CreateAccount Tests

        [Test]
        public async Task CreateAccount_ValidRequest_ReturnsResponseStatus201WithAccountTest()
        {
            // Arrange
            var request = new CreateAccountRequest
            {
                AccountNumber = _accountNumber,
                Currency = _currency,
                InitialBalance = _balance
            };
            var expectedAccount = new Account(request.AccountNumber, request.InitialBalance, request.Currency);
            _accountServiceMock.Setup(srv => srv.CreateAccountAsync(_accountNumber, _balance, _currency))
                                  .ReturnsAsync(expectedAccount);

            // Act
            var actualResponse = await _accountController.CreateAccount(request) as ActionResult;

            // Assert
            Assert.IsNotNull(actualResponse);
            Assert.IsInstanceOf<CreatedAtActionResult>(actualResponse);
            var actualTypeResponse = actualResponse as CreatedAtActionResult;
            Assert.That(actualTypeResponse.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
            var actualBodyResponse = actualTypeResponse.Value as Account;
            AssertAccounts(expectedAccount, actualBodyResponse);
            _accountServiceMock.Verify(srv => srv.CreateAccountAsync(It.IsAny<string>(),
                It.IsAny<decimal>(), It.IsAny<Currency>()), Times.Once);
        }
        
        [Test]
        public async Task CreateAccount_AccountWithExistingNumber_ReturnsResponseStatus400Test()
        {
            // Arrange
            var request = new CreateAccountRequest
            {
                AccountNumber = _accountNumber,
                Currency = _currency,
                InitialBalance = _balance
            };
            _accountServiceMock.Setup(srv => srv.CreateAccountAsync(_accountNumber, _balance, _currency))
                                  .ThrowsAsync(new ArgumentException("An account with this number has already been created."));

            // Act
            var actualResponse = await _accountController.CreateAccount(request) as ActionResult;

            // Assert
            Assert.IsNotNull(actualResponse);
            Assert.IsInstanceOf<BadRequestObjectResult>(actualResponse);
            var actualTypeResponse = actualResponse as BadRequestObjectResult;
            Assert.That(actualTypeResponse.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            _accountServiceMock.Verify(srv => srv.CreateAccountAsync(It.IsAny<string>(),
                It.IsAny<decimal>(), It.IsAny<Currency>()), Times.Once);
        }

        #endregion

        #region Deposit Tests

        [Test]
        public async Task Deposit_ValidRequest_ReturnsResponseStatus200Test()
        {
            // Arrange
            var amount = 50;
            var request = new BalanceRequest
            {
                AccountNumber = _accountNumber,
                Amount = amount
            };
            var expectedBodyResponse = "Deposit successful";
            _accountServiceMock.Setup(srv => srv.DepositAsync(_accountNumber, amount));

            // Act
            var actualResponse = await _accountController.Deposit(request) as ActionResult;

            // Assert
            Assert.IsNotNull(actualResponse);
            Assert.IsInstanceOf<OkObjectResult>(actualResponse);
            var actualTypeResponse = actualResponse as OkObjectResult;
            Assert.That(actualTypeResponse.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(actualTypeResponse.Value, Is.EqualTo(expectedBodyResponse));
            _accountServiceMock.Verify(srv => srv.DepositAsync(It.IsAny<string>(),
                It.IsAny<decimal>()), Times.Once);
        }

        [Test]
        public async Task Deposit_NonExistingAccount_ReturnsResponseStatus400Test()
        {
            // Arrange
            var amount = 50;
            var request = new BalanceRequest
            {
                AccountNumber = _accountNumber,
                Amount = amount
            };
            var expectedBodyResponse = "Account not found.";
            _accountServiceMock.Setup(srv => srv.DepositAsync(_accountNumber, amount))
                .ThrowsAsync(new ArgumentException("Account not found."));

            // Act
            var actualResponse = await _accountController.Deposit(request) as ActionResult;

            // Assert
            Assert.IsNotNull(actualResponse);
            Assert.IsInstanceOf<BadRequestObjectResult>(actualResponse);
            var actualTypeResponse = actualResponse as BadRequestObjectResult;
            Assert.That(actualTypeResponse.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(actualTypeResponse.Value, Is.EqualTo(expectedBodyResponse));
            _accountServiceMock.Verify(srv => srv.DepositAsync(It.IsAny<string>(),
                It.IsAny<decimal>()), Times.Once);
        }

        [Test]
        public async Task Deposit_NegativeAmount_ReturnsResponseStatus400Test()
        {
            // Arrange
            var amount = -50;
            var request = new BalanceRequest
            {
                AccountNumber = _accountNumber,
                Amount = amount
            };
            var expectedBodyResponse = "Deposit amount must be positive.";
            _accountServiceMock.Setup(srv => srv.DepositAsync(_accountNumber, amount))
                .ThrowsAsync(new ArgumentException("Deposit amount must be positive."));

            // Act
            var actualResponse = await _accountController.Deposit(request) as ActionResult;

            // Assert
            Assert.IsNotNull(actualResponse);
            Assert.IsInstanceOf<BadRequestObjectResult>(actualResponse);
            var actualTypeResponse = actualResponse as BadRequestObjectResult;
            Assert.That(actualTypeResponse.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(actualTypeResponse.Value, Is.EqualTo(expectedBodyResponse));
            _accountServiceMock.Verify(srv => srv.DepositAsync(It.IsAny<string>(),
                It.IsAny<decimal>()), Times.Once);
        }

        #endregion

        #region Withdraw Tests

        [Test]
        public async Task Withdraw_ValidRequest_ReturnsResponseStatus200Test()
        {
            // Arrange
            var amount = 50;
            var request = new BalanceRequest
            {
                AccountNumber = _accountNumber,
                Amount = amount
            };
            var expectedBodyResponse = "Withdrawal successful";
            _accountServiceMock.Setup(srv => srv.WithdrawAsync(_accountNumber, amount));

            // Act
            var actualResponse = await _accountController.Withdraw(request) as ActionResult;

            // Assert
            Assert.IsNotNull(actualResponse);
            Assert.IsInstanceOf<OkObjectResult>(actualResponse);
            var actualTypeResponse = actualResponse as OkObjectResult;
            Assert.That(actualTypeResponse.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(actualTypeResponse.Value, Is.EqualTo(expectedBodyResponse));
            _accountServiceMock.Verify(srv => srv.WithdrawAsync(It.IsAny<string>(),
                It.IsAny<decimal>()), Times.Once);
        }

        [Test]
        public async Task Withdraw_NonExistingAccount_ReturnsResponseStatus400Test()
        {
            // Arrange
            var amount = 50;
            var request = new BalanceRequest
            {
                AccountNumber = _accountNumber,
                Amount = amount
            };
            var expectedBodyResponse = "Account not found.";
            _accountServiceMock.Setup(srv => srv.WithdrawAsync(_accountNumber, amount))
                .ThrowsAsync(new ArgumentException("Account not found."));

            // Act
            var actualResponse = await _accountController.Withdraw(request) as ActionResult;

            // Assert
            Assert.IsNotNull(actualResponse);
            Assert.IsInstanceOf<BadRequestObjectResult>(actualResponse);
            var actualTypeResponse = actualResponse as BadRequestObjectResult;
            Assert.That(actualTypeResponse.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(actualTypeResponse.Value, Is.EqualTo(expectedBodyResponse));
            _accountServiceMock.Verify(srv => srv.WithdrawAsync(It.IsAny<string>(),
                It.IsAny<decimal>()), Times.Once);
        }

        [Test]
        public async Task Withdraw_NegativeAmount_ReturnsResponseStatus400Test()
        {
            // Arrange
            var amount = -50;
            var request = new BalanceRequest
            {
                AccountNumber = _accountNumber,
                Amount = amount
            };
            var expectedBodyResponse = "Withdrawal amount must be positive.";
            _accountServiceMock.Setup(srv => srv.WithdrawAsync(_accountNumber, amount))
                .ThrowsAsync(new ArgumentException("Withdrawal amount must be positive."));

            // Act
            var actualResponse = await _accountController.Withdraw(request) as ActionResult;

            // Assert
            Assert.IsNotNull(actualResponse);
            Assert.IsInstanceOf<BadRequestObjectResult>(actualResponse);
            var actualTypeResponse = actualResponse as BadRequestObjectResult;
            Assert.That(actualTypeResponse.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(actualTypeResponse.Value, Is.EqualTo(expectedBodyResponse));
            _accountServiceMock.Verify(srv => srv.WithdrawAsync(It.IsAny<string>(),
                It.IsAny<decimal>()), Times.Once);
        }

        #endregion

        #region Transfer Tests

        [Test]
        public async Task Transfer_ValidRequest_ReturnsResponseStatus200Test()
        {
            // Arrange
            var amount = 50;
            var secondAccountNumber = "8900787854778888";
            var request = new TranferBalanceRequest
            {
                FromAccountNumber = _accountNumber,
                ToAccountNumber = secondAccountNumber,
                Amount = amount
            };
            var expectedBodyResponse = "Transfer successful";
            _accountServiceMock.Setup(srv => srv.TransferAsync(_accountNumber, secondAccountNumber, amount));

            // Act
            var actualResponse = await _accountController.Transfer(request) as ActionResult;

            // Assert
            Assert.IsNotNull(actualResponse);
            Assert.IsInstanceOf<OkObjectResult>(actualResponse);
            var actualTypeResponse = actualResponse as OkObjectResult;
            Assert.That(actualTypeResponse.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(actualTypeResponse.Value, Is.EqualTo(expectedBodyResponse));
            _accountServiceMock.Verify(srv => srv.TransferAsync(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<decimal>()), Times.Once);
        }

        [Test]
        public async Task Transfer_NonExistingSourceAccount_ReturnsResponseStatus400Test()
        {
            // Arrange
            var amount = 50;
            var secondAccountNumber = "8900787854778888";
            var request = new TranferBalanceRequest
            {
                FromAccountNumber = _accountNumber,
                ToAccountNumber = secondAccountNumber,
                Amount = amount
            };
            var expectedBodyResponse = "Source account not found.";
            _accountServiceMock.Setup(srv => srv.TransferAsync(_accountNumber, secondAccountNumber, amount))
                .ThrowsAsync(new ArgumentException("Source account not found."));

            // Act
            var actualResponse = await _accountController.Transfer(request) as ActionResult;

            // Assert
            Assert.IsNotNull(actualResponse);
            Assert.IsInstanceOf<BadRequestObjectResult>(actualResponse);
            var actualTypeResponse = actualResponse as BadRequestObjectResult;
            Assert.That(actualTypeResponse.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(actualTypeResponse.Value, Is.EqualTo(expectedBodyResponse));
            _accountServiceMock.Verify(srv => srv.TransferAsync(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<decimal>()), Times.Once);
        }

        #endregion

        private void AssertAccounts(Account expectedAccount, Account actualAccount)
        {
            Assert.That(expectedAccount.AccountNumber, Is.EqualTo(actualAccount.AccountNumber));
            Assert.That(expectedAccount.Balance, Is.EqualTo(actualAccount.Balance));
            Assert.That(expectedAccount.Currency, Is.EqualTo(actualAccount.Currency));
        }
    }
}