namespace MyWebAPI.Models
{
    public class TransactionDto
    {
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int TransactionType {get; set;}
        public string Description { get; set; }

        public int AccountId { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
    }
}