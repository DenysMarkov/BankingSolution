using BankingSolution.Application.Services;
using BankingSolution.Domain.Entities;
using BankingSolution.Domain.Interfaces;
using Moq;

namespace BankingSolution.Tests
{
    [TestFixture]
    public class AccountServiceTests
    {
        private Mock<IAccountRepository> _accountRepositoryMock;
        private AccountService _accountService;
        private string _accountNumber;
        private Currency _currency;
        private string _accountNumberSender;
        private string _accountNumberReceiver;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _accountNumber = "1200457721415111";
            _accountNumberSender = _accountNumber;
            _accountNumberReceiver = "8900787854778888";
            _currency = Currency.USD;
        }

        [SetUp]
        public void Setup()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _accountService = new AccountService(_accountRepositoryMock.Object);
        }

        #region CreateAccountAsync Tests

        [Test]
        public async Task CreateAccountAsync_ShouldCreateAccountTest()
        {
            // Arrange
            var initialBalance = 1000m;

            // Act
            var account = await _accountService.CreateAccountAsync(_accountNumber, initialBalance, _currency);

            // Assert
            Assert.That(_accountNumber, Is.EqualTo(account.AccountNumber));
            Assert.That(initialBalance, Is.EqualTo(account.Balance));
            _accountRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Account>()), Times.Once);
            _accountRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task CreateAccountAsync_ShouldThrowException_WhenAccountWithSameNumberAlreadyCreatedTest()
        {
            // Arrange
            var account = new Account(_accountNumber, 100, _currency);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync(_accountNumber))
                                  .ReturnsAsync(account);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _accountService.CreateAccountAsync(_accountNumber, 50, _currency));
        }

        #endregion

        #region GetAccountDetailsAsync Tests

        [Test]
        public async Task GetAccountDetailsAsync_ExistingAccount_ReturnsAccountTest()
        {
            // Arrange
            var account = new Account(_accountNumber, 1000m, _currency);
            _accountRepositoryMock
                .Setup(repo => repo.GetByAccountNumberAsync(_accountNumber))
                .ReturnsAsync(account);

            // Act
            var result = await _accountService.GetAccountDetailsAsync(_accountNumber);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.AccountNumber, Is.EqualTo(_accountNumber));
            Assert.That(result.Balance, Is.EqualTo(1000m));
        }

        [Test]
        public async Task GetAccountDetailsAsync_NonExistingAccount_NullReturnedTest()
        {
            // Arrange
            _accountRepositoryMock
                .Setup(repo => repo.GetByAccountNumberAsync(_accountNumber))
                .ReturnsAsync((Account)null);

            // Act
            var result = await _accountService.GetAccountDetailsAsync(_accountNumber);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region GetAllAccountsAsync Tests

        [Test]
        public async Task GetAllAccountsAsync_ReturnsListOfAccountsTest()
        {
            // Arrange
            var secondAccountNumber = _accountNumber + "0";
            var accounts = new List<Account>
            {
                new Account(_accountNumber, 1000m, _currency),
                new Account(secondAccountNumber, 500m, _currency)
            };
            _accountRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(accounts);

            // Act
            var result = await _accountService.GetAllAccountsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.That(accounts.Count, Is.EqualTo(result.Count()));
            Assert.IsTrue(result.Any(a => a.AccountNumber == accounts[0].AccountNumber && a.Balance == accounts[0].Balance));
            Assert.IsTrue(result.Any(a => a.AccountNumber == accounts[1].AccountNumber && a.Balance == accounts[1].Balance));
        }

        [Test]
        public async Task GetAllAccountsAsync_EmptyRepository_ReturnsEmptyListTest()
        {
            // Arrange
            _accountRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Account>());

            // Act
            var result = await _accountService.GetAllAccountsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        #endregion

        #region DepositAsync Tests

        [Test]
        public async Task DepositAsync_ShouldIncreaseBalance_WhenValidAmountProvidedTest()
        {
            // Arrange
            var balance = 100;
            var amount = 50;
            var expectedBalance = balance + amount;
            var account = new Account(_accountNumber, balance, _currency);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync(_accountNumber))
                                  .ReturnsAsync(account);

            // Act
            await _accountService.DepositAsync(_accountNumber, amount);

            // Assert
            Assert.That(account.Balance, Is.EqualTo(expectedBalance));
            _accountRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DepositAsync_ShouldThrowException_WhenAccountNotFoundTest()
        {
            // Arrange
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync(_accountNumber))
                                  .ReturnsAsync((Account)null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _accountService.DepositAsync(_accountNumber, 50));
        }

        [Test]
        public void DepositAsync_ShouldThrowException_WhenAmountIsNegativeTest()
        {
            // Arrange
            var amount = -10;
            var account = new Account(_accountNumber, 100, _currency);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync(_accountNumber))
                                  .ReturnsAsync(account);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _accountService.DepositAsync(_accountNumber, amount));
        }

        #endregion

        #region WithdrawAsync Tests

        [Test]
        public async Task WithdrawAsync_ShouldDecreaseBalance_WhenSufficientFundsTest()
        {
            // Arrange
            var balance = 100;
            var amount = 50;
            var expectedBalance = balance - amount;
            var account = new Account(_accountNumber, balance, _currency);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync(_accountNumber))
                                  .ReturnsAsync(account);

            // Act
            await _accountService.WithdrawAsync(_accountNumber, amount);

            // Assert
            Assert.That(account.Balance, Is.EqualTo(expectedBalance));
            _accountRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void WithdrawAsync_ShouldThrowException_WhenAccountNotFoundTest()
        {
            // Arrange
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync(_accountNumber))
                                  .ReturnsAsync((Account)null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _accountService.WithdrawAsync(_accountNumber, 50));
        }

        [Test]
        public void WithdrawAsync_ShouldThrowException_WhenAmountIsNegativeTest()
        {
            // Arrange
            var amount = -10;
            var account = new Account(_accountNumber, 100, _currency);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync(_accountNumber))
                                  .ReturnsAsync(account);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _accountService.WithdrawAsync(_accountNumber, amount));
        }

        [Test]
        public void WithdrawAsync_ShouldThrowException_WhenInsufficientFundsTest()
        {
            // Arrange
            var balance = 20;
            var amount = 50;
            var account = new Account(_accountNumber, balance, _currency);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync(_accountNumber))
                                  .ReturnsAsync(account);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _accountService.WithdrawAsync(_accountNumber, amount));
        }

        #endregion

        #region TransferAsync Tests

        [Test]
        public async Task TransferAsync_ShouldTransferFunds_WhenSufficientBalanceTest()
        {
            // Arrange
            var balanceSender = 100;
            var balanceReceiver = 50;
            var amount = 50;
            var expectedBalanceSender = balanceSender - amount;
            var expectedBalanceReceiver = balanceReceiver + amount;
            var sender = new Account(_accountNumberSender, balanceSender, _currency);
            var receiver = new Account(_accountNumberReceiver, balanceReceiver, _currency);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync(_accountNumberSender))
                                  .ReturnsAsync(sender);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync(_accountNumberReceiver))
                                  .ReturnsAsync(receiver);

            // Act
            await _accountService.TransferAsync(_accountNumberSender, _accountNumberReceiver, amount);

            // Assert
            Assert.That(sender.Balance, Is.EqualTo(expectedBalanceSender));
            Assert.That(receiver.Balance, Is.EqualTo(expectedBalanceReceiver));
            _accountRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void TransferAsync_ShouldThrowException_WhenSenderNotFoundTest()
        {
            // Arrange
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync(_accountNumberSender))
                                  .ReturnsAsync((Account)null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _accountService.TransferAsync(_accountNumberSender, _accountNumberReceiver, 50));
        }

        [Test]
        public void TransferAsync_ShouldThrowException_WhenReceiverNotFoundTest()
        {
            // Arrange
            var sender = new Account(_accountNumberSender, 100, _currency);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync(_accountNumberSender))
                                  .ReturnsAsync(sender);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync(_accountNumberReceiver))
                                  .ReturnsAsync((Account)null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _accountService.TransferAsync(_accountNumberSender, _accountNumberReceiver, 50));
        }

        [Test]
        public void TransferAsync_ShouldThrowException_WhenInsufficientFundsTest()
        {
            // Arrange
            var balanceSender = 20;
            var balanceReceiver = 50;
            var amount = 50;
            var sender = new Account(_accountNumberSender, balanceSender, _currency);
            var receiver = new Account(_accountNumberReceiver, balanceReceiver, _currency);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync(_accountNumberSender))
                                  .ReturnsAsync(sender);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync(_accountNumberReceiver))
                                  .ReturnsAsync(receiver);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _accountService.TransferAsync(_accountNumberSender, _accountNumberReceiver, amount));
        }

        [Test]
        public void TransferAsync_ShouldThrowException_WhenDifferentCurrenciesInAccountsTest()
        {
            // Arrange
            var balanceSender = 100;
            var balanceReceiver = 50;
            var amount = 50;
            var anotherCurrency = Currency.EUR;
            var sender = new Account(_accountNumberSender, balanceSender, _currency);
            var receiver = new Account(_accountNumberReceiver, balanceReceiver, anotherCurrency);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync(_accountNumberSender))
                                  .ReturnsAsync(sender);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync(_accountNumberReceiver))
                                  .ReturnsAsync(receiver);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _accountService.TransferAsync(_accountNumberSender, _accountNumberReceiver, amount));
        }

        #endregion
    }
}