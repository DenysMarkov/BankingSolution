using BankingSolution.Domain.Entities;

namespace BankingSolution.Domain.Interfaces
{
    public interface IAccount
    {
        string AccountNumber { get; }
        decimal Balance { get; }
        Currency Currency { get; }
        void Deposit(decimal amount);
        public void Withdraw(decimal amount);
    }
}
