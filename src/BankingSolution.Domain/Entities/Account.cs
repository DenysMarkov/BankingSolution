using BankingSolution.Domain.Interfaces;

namespace BankingSolution.Domain.Entities
{
    /// <summary>
    /// Represents a bank account in the system.
    /// </summary>
    public class Account : IAccount
    {
        public string AccountNumber { get; private set; }
        public decimal Balance { get; private set; }
        public Currency Currency { get; private set; }

        public Account(string accountNumber, decimal balance, Currency currency)
        {
            if (string.IsNullOrEmpty(accountNumber))
            {
                throw new ArgumentNullException(nameof(accountNumber));
            }
            if (balance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(balance));
            }

            AccountNumber = accountNumber;
            Balance = balance;
            Currency = currency;
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
