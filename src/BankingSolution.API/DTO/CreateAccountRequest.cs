using BankingSolution.Domain.Entities;
using System.Text.Json.Serialization;

namespace BankingSolution.API.DTO
{
    public class CreateAccountRequest
    {
        public string AccountNumber { get; set; }
        
        public decimal Amount { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Currency Currency { get; set; }
    }
}
