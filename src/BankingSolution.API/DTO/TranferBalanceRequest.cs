namespace BankingSolution.API.DTO
{
    /// <summary>
    /// Data Transfer Object for Transfer operation.
    /// </summary>
    public class TranferBalanceRequest
    {
        public string FromAccountNumber { get; set; }
        public string ToAccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
