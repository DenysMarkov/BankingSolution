﻿using BankingSolution.Application.Interfaces;
using BankingSolution.Domain.Entities;
using BankingSolution.Domain.Interfaces;

namespace BankingSolution.Application.Services
{
    /// <summary>
    /// Service for handling account-related operations.
    /// </summary>
    public class AccountService(IAccountRepository _accountRepository) : IAccountService
    {
        public async Task<Account> CreateAccountAsync(string accountNumber, decimal initialBalance, Currency currency)
        {
            var createdAccount = await _accountRepository.GetByAccountNumberAsync(accountNumber);
            if (createdAccount != null)
            {
                throw new ArgumentException("An account with this number has already been created.");
            }

            var account = new Account(accountNumber, initialBalance, currency);

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

            if (fromAccount.Currency != toAccount.Currency)
            {
                throw new InvalidOperationException("Operation of transferring funds between accounts with different currencies.");
            }

            try
            {
                fromAccount.Withdraw(amount);
            }
            catch(ArgumentException ex)
            {
                throw new ArgumentException("Transfer amount must be positive.");
            }
            toAccount.Deposit(amount);

            await _accountRepository.SaveChangesAsync();
        }
    }
}
