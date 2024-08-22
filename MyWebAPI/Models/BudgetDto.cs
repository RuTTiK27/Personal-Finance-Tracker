namespace MyWebAPI.Models
{
    public class BudgetDto
    {
        public int BudgetId { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
    }
}