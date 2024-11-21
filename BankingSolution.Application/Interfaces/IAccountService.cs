using BankingSolution.Domain.Entities;

namespace BankingSolution.Application.Interfaces
{
    /// <summary>
    /// Interface for AccountService operations.
    /// </summary>
    public interface IAccountService
    {
        Task<Account> CreateAccountAsync(string accountNumber, decimal initialBalance, Currency currency);
        Task<Account> GetAccountDetailsAsync(string accountNumber);
        Task<IEnumerable<Account>> GetAllAccountsAsync();
        Task DepositAsync(string accountNumber, decimal amount);
        Task WithdrawAsync(string accountNumber, decimal amount);
        Task TransferAsync(string fromAccountNumber, string toAccountNumber, decimal amount);
    }
}
