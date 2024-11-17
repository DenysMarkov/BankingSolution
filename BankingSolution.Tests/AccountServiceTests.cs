using BankingSolution.Application.Interfaces;
using BankingSolution.Application.Services;
using BankingSolution.Domain.Entities;
using Moq;

namespace BankingSolution.Tests
{
    [TestFixture]
    public class AccountServiceTests
    {
        private Mock<IAccountRepository> _accountRepositoryMock;
        private AccountService _accountService;

        [SetUp]
        public void Setup()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _accountService = new AccountService(_accountRepositoryMock.Object);
        }

        [Test]
        public async Task CreateAccountAsync_ShouldCreateAccount()
        {
            // Arrange
            var accountNumber = "123456";
            var initialBalance = 1000m;

            // Act
            var account = await _accountService.CreateAccountAsync(accountNumber, initialBalance);

            // Assert
            Assert.That(accountNumber, Is.EqualTo(account.AccountNumber));
            Assert.That(initialBalance, Is.EqualTo(account.Balance));
            _accountRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Account>()), Times.Once);
            _accountRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task DepositAsync_ShouldIncreaseBalance_WhenValidAmountProvided()
        {
            // Arrange
            var account = new Account("12345", 100);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync("12345"))
                                  .ReturnsAsync(account);

            // Act
            await _accountService.DepositAsync("12345", 50);

            // Assert
            Assert.AreEqual(150, account.Balance);
            _accountRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DepositAsync_ShouldThrowException_WhenAccountNotFound()
        {
            // Arrange
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync("12345"))
                                  .ReturnsAsync((Account)null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _accountService.DepositAsync("12345", 50));
        }

        [Test]
        public async Task WithdrawAsync_ShouldDecreaseBalance_WhenSufficientFunds()
        {
            // Arrange
            var account = new Account("12345", 100);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync("12345"))
                                  .ReturnsAsync(account);

            // Act
            await _accountService.WithdrawAsync("12345", 50);

            // Assert
            Assert.AreEqual(50, account.Balance);
            _accountRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void WithdrawAsync_ShouldThrowException_WhenInsufficientFunds()
        {
            // Arrange
            var account = new Account("12345", 30);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync("12345"))
                                  .ReturnsAsync(account);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _accountService.WithdrawAsync("12345", 50));
        }

        [Test]
        public void WithdrawAsync_ShouldThrowException_WhenAccountNotFound()
        {
            // Arrange
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync("12345"))
                                  .ReturnsAsync((Account)null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _accountService.WithdrawAsync("12345", 50));
        }

        [Test]
        public async Task TransferAsync_ShouldTransferFunds_WhenSufficientBalance()
        {
            // Arrange
            var sender = new Account("12345", 100);
            var receiver = new Account("67890", 50);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync("12345"))
                                  .ReturnsAsync(sender);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync("67890"))
                                  .ReturnsAsync(receiver);

            // Act
            await _accountService.TransferAsync("12345", "67890", 50);

            // Assert
            Assert.AreEqual(50, sender.Balance);
            Assert.AreEqual(100, receiver.Balance);
            _accountRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Exactly(1));
        }

        [Test]
        public void TransferAsync_ShouldThrowException_WhenSenderNotFound()
        {
            // Arrange
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync("12345"))
                                  .ReturnsAsync((Account)null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _accountService.TransferAsync("12345", "67890", 50));
        }

        [Test]
        public void TransferAsync_ShouldThrowException_WhenReceiverNotFound()
        {
            // Arrange
            var sender = new Account("12345", 100);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync("12345"))
                                  .ReturnsAsync(sender);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync("67890"))
                                  .ReturnsAsync((Account)null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _accountService.TransferAsync("12345", "67890", 50));
        }

        [Test]
        public void TransferAsync_ShouldThrowException_WhenInsufficientFunds()
        {
            // Arrange
            var sender = new Account("12345", 30);
            var receiver = new Account("67890", 50);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync("12345"))
                                  .ReturnsAsync(sender);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync("67890"))
                                  .ReturnsAsync(receiver);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _accountService.TransferAsync("12345", "67890", 50));
        }

        [Test]
        public void DepositAsync_ShouldThrowException_WhenAmountIsNegative()
        {
            // Arrange
            var account = new Account("12345", 100);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync("12345"))
                                  .ReturnsAsync(account);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _accountService.DepositAsync("12345", -10));
        }

        [Test]
        public void WithdrawAsync_ShouldThrowException_WhenAmountIsNegative()
        {
            // Arrange
            var account = new Account("12345", 100);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync("12345"))
                                  .ReturnsAsync(account);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _accountService.WithdrawAsync("12345", -10));
        }
    }
}