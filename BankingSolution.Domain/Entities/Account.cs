using BankingSolution.Domain.Interfaces;

namespace BankingSolution.Domain.Entities
{
    /// <summary>
    /// Represents a bank account in the system.
    /// </summary>
    public class Account : IAccount
    {
        public Guid Id { get; private set; }
        public string AccountNumber { get; private set; }
        public decimal Balance { get; private set; }

        public Account(string accountNumber, decimal balance)
        {
            Id = Guid.NewGuid();
            AccountNumber = accountNumber;
            Balance = balance;
        }

        /// <summary>
        /// Deposits funds into the account.
        /// </summary>
        public void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Deposit amount must be positive.");
            }
            Balance += amount;
        }

        /// <summary>
        /// Withdraws funds from the account.
        /// </summary>
        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Withdrawal amount must be positive.");
            }
            if (Balance < amount)
            {
                throw new InvalidOperationException("Insufficient funds.");
            }
            Balance -= amount;
        }
    }
}
