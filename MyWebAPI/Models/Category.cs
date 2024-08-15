namespace MyWebAPI.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description{get;set;}
        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<Budget> Budgets { get; set; } 
        public ICollection<Transaction> Transactions { get; set; }
    }
}
