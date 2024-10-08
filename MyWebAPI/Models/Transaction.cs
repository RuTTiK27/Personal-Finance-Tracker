using System.Text.Json.Serialization;

namespace MyWebAPI.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int TransactionType {get; set;}
        public string Description { get; set; }

        public int AccountId { get; set; }
        [JsonIgnore]
        public Account Account { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

    }
}
