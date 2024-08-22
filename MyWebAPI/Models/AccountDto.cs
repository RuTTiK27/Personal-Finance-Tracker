namespace MyWebAPI.Models
{
    public class AccountDto
    {
        public string? AccountName { get; set; }
        public int? AccountType { get; set; }
        public decimal? Balance { get; set; }
        public int UserId { get; set; }  // Only UserId needed for the request
    }
}
