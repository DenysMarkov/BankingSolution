using BankingSolution.API.DTO;
using BankingSolution.Application.Interfaces;
using BankingSolution.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BankingSolution.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController(IAccountService _accountService, ILogger<AccountController> _logger) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllAccounts()
        {
            var accounts = await _accountService.GetAllAccountsAsync();

            return Ok(accounts);
        }

        [HttpGet("{accountNumber}")]
        public async Task<IActionResult> GetAccountDetails(string accountNumber)
        {
            var account = await _accountService.GetAccountDetailsAsync(accountNumber);
            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Account account;

            try
            {
                account = await _accountService.CreateAccountAsync(request.AccountNumber, request.InitialBalance, request.Currency);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return CreatedAtAction(nameof(GetAccountDetails), new { accountNumber = account.AccountNumber }, account);
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] BalanceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _accountService.DepositAsync(request.AccountNumber, request.Amount);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("Deposit successful");
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] BalanceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _accountService.WithdrawAsync(request.AccountNumber, request.Amount);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("Withdrawal successful");
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TranferBalanceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _accountService.TransferAsync(request.FromAccountNumber, request.ToAccountNumber, request.Amount);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("Transfer successful");
        }
    }
}
