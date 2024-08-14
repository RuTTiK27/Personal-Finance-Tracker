namespace MyWebAPI.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<Budget> Budgets { get; set; } 
    }
}
