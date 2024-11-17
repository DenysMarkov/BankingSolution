using BankingSolution.Domain.Entities;
using BankingSolution.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingSolution.Infrastructure
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BankingDbContext _dbContext;

        public AccountRepository(BankingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

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
