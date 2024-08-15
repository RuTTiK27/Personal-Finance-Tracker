namespace MyWebAPI.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedDate {get; set;}

        public ICollection<Account> Accounts { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<Budget> Budgets { get; set; } 
        public ICollection<Category> Categories { get; set; }
    }
}
