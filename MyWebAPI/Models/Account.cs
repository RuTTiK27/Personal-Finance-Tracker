namespace MyWebAPI.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public int AccountType {get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedDate { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
