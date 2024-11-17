namespace BankingSolution.Domain.Interfaces
{
    public interface IAccount
    {
        Guid Id { get; }
        string AccountNumber { get; }
        decimal Balance { get; }
        void Deposit(decimal amount);
        public void Withdraw(decimal amount);
    }
}
