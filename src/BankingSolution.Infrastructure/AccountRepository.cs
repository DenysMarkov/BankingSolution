using BankingSolution.Domain.Entities;
using BankingSolution.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingSolution.Infrastructure
{
    public class AccountRepository(BankingDbContext _dbContext) : IAccountRepository
    {

        public async Task<Account> GetByAccountNumberAsync(string accountNumber) =>
            await _dbContext.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

        public async Task<IEnumerable<Account>> GetAllAsync() =>
            await _dbContext.Accounts.ToListAsync();

        public async Task AddAsync(Account account) =>
            await _dbContext.Accounts.AddAsync(account);

        public async Task SaveChangesAsync() =>
            await _dbContext.SaveChangesAsync();
    }
}
