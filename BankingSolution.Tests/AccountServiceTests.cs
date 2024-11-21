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

        #region CreateAccountAsync

        [Test]
        public async Task CreateAccountAsync_ShouldCreateAccount()
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
        public async Task CreateAccountAsync_ShouldThrowException_WhenAccountWithSameNumberAlreadyCreated()
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

        #region GetAccountDetailsAsync

        [Test]
        public async Task GetAccountDetailsAsync_ExistingAccount_ReturnsAccount()
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
        public async Task GetAccountDetailsAsync_NonExistingAccount_NullReturned()
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

        #region GetAllAccountsAsync

        [Test]
        public async Task GetAllAccountsAsync_ReturnsListOfAccounts()
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
        public async Task GetAllAccountsAsync_EmptyRepository_ReturnsEmptyList()
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

        #region DepositAsync

        [Test]
        public async Task DepositAsync_ShouldIncreaseBalance_WhenValidAmountProvided()
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
        public void DepositAsync_ShouldThrowException_WhenAccountNotFound()
        {
            // Arrange
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync(_accountNumber))
                                  .ReturnsAsync((Account)null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _accountService.DepositAsync(_accountNumber, 50));
        }

        [Test]
        public void DepositAsync_ShouldThrowException_WhenAmountIsNegative()
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

        #region WithdrawAsync

        [Test]
        public async Task WithdrawAsync_ShouldDecreaseBalance_WhenSufficientFunds()
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
        public void WithdrawAsync_ShouldThrowException_WhenAccountNotFound()
        {
            // Arrange
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync(_accountNumber))
                                  .ReturnsAsync((Account)null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _accountService.WithdrawAsync(_accountNumber, 50));
        }

        [Test]
        public void WithdrawAsync_ShouldThrowException_WhenAmountIsNegative()
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
        public void WithdrawAsync_ShouldThrowException_WhenInsufficientFunds()
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

        #region TransferAsync

        [Test]
        public async Task TransferAsync_ShouldTransferFunds_WhenSufficientBalance()
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
        public void TransferAsync_ShouldThrowException_WhenSenderNotFound()
        {
            // Arrange
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync(_accountNumberSender))
                                  .ReturnsAsync((Account)null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _accountService.TransferAsync(_accountNumberSender, _accountNumberReceiver, 50));
        }

        [Test]
        public void TransferAsync_ShouldThrowException_WhenReceiverNotFound()
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
        public void TransferAsync_ShouldThrowException_WhenInsufficientFunds()
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
        public void TransferAsync_ShouldThrowException_WhenDifferentCurrenciesInAccounts()
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