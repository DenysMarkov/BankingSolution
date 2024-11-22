using BankingSolution.Domain.Entities;
using System.Text.Json.Serialization;

namespace BankingSolution.API.DTO
{
    /// <summary>
    /// Data Transfer Object for Creating account.
    /// </summary>
    public class CreateAccountRequest
    {
        public string AccountNumber { get; set; }
        
        public decimal Amount { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Currency Currency { get; set; }
    }
}
