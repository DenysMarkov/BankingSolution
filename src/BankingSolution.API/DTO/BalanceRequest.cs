namespace BankingSolution.API.DTO
{
    /// <summary>
    /// Data Transfer Object for Deposit and Withdraw operations.
    /// </summary>
    public class BalanceRequest
    {
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
