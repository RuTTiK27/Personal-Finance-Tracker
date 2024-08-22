using System.Collections.Generic; // Make sure to include this for ICollection
using System.ComponentModel.DataAnnotations;

namespace MyWebAPI.Models
{
    public class Account
    {
        public int AccountId { get; set; }

        [Required]
        public string AccountName { get; set; }
        
        [Required]
        public int AccountType {get; set; }
        
        [Required]
        public decimal Balance { get; set; }
        
        public DateTime CreatedDate { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
        
    }
}
