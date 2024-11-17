using BankingSolution.Domain.Entities;

namespace BankingSolution.Application.Interfaces
{
    /// <summary>
    /// Interface for AccountService operations.
    /// </summary>
    public interface IAccountService
    {
        Task<Account> CreateAccountAsync(string accountNumber, decimal initialBalance);
        Task<Account> GetAccountDetailsAsync(string accountNumber);
        Task<IEnumerable<Account>> GetAllAccountsAsync();
        Task TransferAsync(string fromAccountNumber, string toAccountNumber, decimal amount);
    }
}
