using BankingSolution.Domain.Entities;
using BankingSolution.Application.Interfaces;

namespace BankingSolution.Application.Services
{
    /// <summary>
    /// Service for handling account-related operations.
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<Account> CreateAccountAsync(string accountNumber, decimal initialBalance)
        {
            var account = new Account(accountNumber, initialBalance);

            await _accountRepository.AddAsync(account);
            await _accountRepository.SaveChangesAsync();

            return account;
        }

        public async Task<Account> GetAccountDetailsAsync(string accountNumber)
        {
            return await _accountRepository.GetByAccountNumberAsync(accountNumber);
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            return await _accountRepository.GetAllAsync();
        }

        public async Task DepositAsync(string accountNumber, decimal amount)
        {
            var account = await _accountRepository.GetByAccountNumberAsync(accountNumber)
                          ?? throw new ArgumentException("Account not found.");

            account.Deposit(amount);

            await _accountRepository.SaveChangesAsync();
        }

        public async Task WithdrawAsync(string accountNumber, decimal amount)
        {
            var account = await _accountRepository.GetByAccountNumberAsync(accountNumber)
                          ?? throw new ArgumentException("Account not found.");

            account.Withdraw(amount);

            await _accountRepository.SaveChangesAsync();
        }

        public async Task TransferAsync(string fromAccountNumber, string toAccountNumber, decimal amount)
        {
            var fromAccount = await _accountRepository.GetByAccountNumberAsync(fromAccountNumber)
                              ?? throw new ArgumentException("Source account not found.");
            var toAccount = await _accountRepository.GetByAccountNumberAsync(toAccountNumber)
                            ?? throw new ArgumentException("Destination account not found.");

            fromAccount.Withdraw(amount);
            toAccount.Deposit(amount);

            await _accountRepository.SaveChangesAsync();
        }
    }
}
