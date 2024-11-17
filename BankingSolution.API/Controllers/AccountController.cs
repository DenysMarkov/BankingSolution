using Microsoft.AspNetCore.Mvc;
using BankingSolution.API.DTO;
using BankingSolution.Application.Interfaces;

namespace BankingSolution.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountService accountService, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

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
        public async Task<IActionResult> CreateAccount([FromBody] BalanceRequest request)
        {
            var account = await _accountService.CreateAccountAsync(request.AccountNumber, request.Amount);
            return CreatedAtAction(nameof(GetAccountDetails), new { accountNumber = account.AccountNumber }, account);
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] BalanceRequest request)
        {
            try
            {
                await _accountService.DepositAsync(request.AccountNumber, request.Amount);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("Deposit successful");
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] BalanceRequest request)
        {
            try
            {
                await _accountService.WithdrawAsync(request.AccountNumber, request.Amount);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("Withdrawal successful");
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TranferBalanceRequest request)
        {
            try
            {
                await _accountService.TransferAsync(request.FromAccountNumber, request.ToAccountNumber, request.Amount);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("Transfer successful");
        }
    }
}
