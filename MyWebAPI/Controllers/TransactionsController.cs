using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MyWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController:ControllerBase
    {
        private readonly MyAppDbContext _context;

        public TransactionsController(MyAppDbContext context)
        {
            _context = context;//Transaction
            
        }

        //GET: api/Transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            return await _context.Transactions.ToListAsync();
        }

        //GET: api/Transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            return transaction;
        }

        //POST: api/Transactions
        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(TransactionDto  transactionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var transaction = new Transaction
            {
                Amount = transactionDto.Amount,
                Date = DateTime.Now,
                TransactionType = transactionDto.TransactionType,
                Description = transactionDto.Description,

                AccountId = transactionDto.AccountId,
                UserId = transactionDto.UserId,
                CategoryId = transactionDto.CategoryId
            };
            // Add the transaction to the database
            _context.Transactions.Add(transaction);

            // Update the account balance
            var account = await _context.Accounts.FindAsync(transactionDto.AccountId);
            if (account == null)
            {
                return NotFound();
            }

            if (transactionDto.TransactionType == 1)
            {
                account.Balance += transactionDto.Amount;
            }
            else if (transactionDto.TransactionType == 2)
            {
                account.Balance -= transactionDto.Amount;
            }

            // Save changes to both transaction and account
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.TransactionId }, transaction);
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(t => t.TransactionId == id);
        }

        //PUT: api/Users/5
        // [HttpPut("{transactionId}")]
        //ChatGPT Logic Start
        // public async Task<IActionResult> PutTransaction(int transactionId, TransactionDto transactionDto)
        // {
        //     var transaction = await _context.Transactions.FindAsync(transactionId);
        //     if (transaction == null)
        //     {
        //         return NotFound();
        //     }

        //     var account = await _context.Accounts.FindAsync(transaction.AccountId);
        //     if (account != null)
        //     {
        //         // Reverse the effect of the original transaction
        //         if (transaction.TransactionType == 1)
        //         {
        //             account.Balance += transaction.Amount; // Reverse debit
        //         }
        //         else if (transaction.TransactionType == 2)
        //         {
        //             account.Balance -= transaction.Amount; // Reverse credit
        //         }

        //         // Apply the new transaction values
        //         if (transactionDto.TransactionType == 1)
        //         {
        //             account.Balance -= transactionDto.Amount; // Apply new debit
        //         }
        //         else if (transactionDto.TransactionType == 2)
        //         {
        //             account.Balance += transactionDto.Amount; // Apply new credit
        //         }

        //         // Update transaction details
        //         transaction.Amount = transactionDto.Amount;
        //         transaction.TransactionType = transactionDto.TransactionType;
        //         transaction.Description = transactionDto.Description;

        //         await _context.SaveChangesAsync();
        //     }

        //     return NoContent(); // or return Ok(transaction) if you want to return the updated transaction
        // }
        //ChatGpt Logic End

        //My Logic Start
        // public async Task<IActionResult> PutTransaction(int transactionId, TransactionDto transactionDto)
        // {
        //     var transaction = await _context.Transactions.FindAsync(transactionId);
        //     if (transaction == null)
        //     {
        //         return NotFound();
        //     }

        //     var account = await _context.Accounts.FindAsync(transaction.AccountId);
        //     if (account != null)
        //     {
        //         if (transactionDto.TransactionType == 1)
        //         {
        //             account.Balance -= transaction.Amount;
        //         }
        //         else if (transactionDto.TransactionType == 2)
        //         {
        //             account.Balance += transaction.Amount;
        //         }
        //     }
        //     // Update transaction details
        //     transaction.Amount = transactionDto.Amount;
        //     transaction.TransactionType = transactionDto.TransactionType;
        //     transaction.Description = transactionDto.Description;

        //     // Adjust the account balance if needed
        //     if (account != null)
        //     {
        //         if (transactionDto.TransactionType == 1)
        //         {
        //             account.Balance += transactionDto.Amount;
        //         }
        //         else if (transactionDto.TransactionType == 2)
        //         {
        //             account.Balance -= transactionDto.Amount;
        //         }
        //         await _context.SaveChangesAsync();
        //     }

        //     await _context.SaveChangesAsync();

        //     return NoContent(); // or return Ok(transaction) if you want to return the updated transaction
        // }
        //My Logic End

        //DELETE: api/Users/5
        [HttpDelete("{transactionId}")]
        public async Task<IActionResult> DeleteTransaction(int transactionId)
        {
            var transaction = await _context.Transactions.FindAsync(transactionId);
            if (transaction == null)
            {
                return NotFound();
            }

            // Optionally adjust the account balance
            var account = await _context.Accounts.FindAsync(transaction.AccountId);
            if (account != null)
            {
                if (transaction.TransactionType == 1)
                {
                    account.Balance -= transaction.Amount;
                }
                else if (transaction.TransactionType == 2)
                {
                    account.Balance += transaction.Amount;
                }
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}