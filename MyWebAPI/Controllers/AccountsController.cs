using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MyWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController:ControllerBase
    {
        private readonly MyAppDbContext _context;

        public AccountsController(MyAppDbContext context)
        {
            _context = context;
        }

        //GET: api/Accounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return await _context.Accounts.ToListAsync();
        }

        //GET: api/Accounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }

        //POST: api/Accounts
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount(AccountDto accountDto)
        {
            // Validate the DTO
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Map the DTO to the Account model
            var account = new Account
            {
                AccountName = accountDto.AccountName,
                AccountType = accountDto.AccountType ?? 0,  // Use default value if null
                Balance = accountDto.Balance ?? 0,          // Use default value if null
                UserId = accountDto.UserId,
                CreatedDate = DateTime.Now
            };
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAccount), new {id = account.AccountId},account);
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }

        //PUT: api/Accounts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(int id,[FromBody] AccountDto accountDto)
        {
            var account = await _context.Accounts.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            // Update fields only if they are provided
            if (!string.IsNullOrEmpty(accountDto.AccountName))
            {
                account.AccountName = accountDto.AccountName;
            }
            
            if (accountDto.Balance.HasValue)
            {
                account.Balance = accountDto.Balance.Value;
            }            
            
            _context.Entry(account).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        //DELETE: api/Accounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}