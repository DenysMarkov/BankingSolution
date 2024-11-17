using BankingSolution.Domain.Entities;

namespace BankingSolution.Application.Interfaces
{
    /// <summary>
    /// Interface for account data access.
    /// </summary>
    public interface IAccountRepository
    {
        Task<Account> GetByAccountNumberAsync(string accountNumber);
        Task<IEnumerable<Account>> GetAllAsync();
        Task AddAsync(Account account);
        Task SaveChangesAsync();
    }
}
