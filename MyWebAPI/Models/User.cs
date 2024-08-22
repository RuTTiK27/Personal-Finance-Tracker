using System.Collections.Generic; // Make sure to include this for ICollection
using System.ComponentModel.DataAnnotations;

namespace MyWebAPI.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        
        public DateTime CreatedDate {get; set;}
        
        
        public ICollection<Account> Accounts { get; set; } = new HashSet<Account>();
        public ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
        public ICollection<Budget> Budgets { get; set; } = new HashSet<Budget>();
        public ICollection<Category> Categories { get; set; } = new HashSet<Category>();
    }
}
