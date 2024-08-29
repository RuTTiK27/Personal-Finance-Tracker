using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MyWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetController:ControllerBase
    {
        private readonly MyAppDbContext _context;

        public BudgetController(MyAppDbContext context)
        {
            _context = context;
        }

        //GET: api/Budgets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Budget>>> GetBudgets()
        {
            return await _context.Budgets.ToListAsync();
        }

        //GET: api/Budgets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Budget>> GetBudget(int id)
        {
            var budget = await _context.Budgets.FindAsync(id);

            if (budget == null)
            {
                return NotFound();
            }

            return budget;
        }

        //POST: api/Budgets
        [HttpPost]
        public async Task<ActionResult<Budget>> PostBudget(BudgetDto budgetDto)
        {
            // Validate the DTO
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Map the DTO to the Budget model
            var budget = new Budget
            {
                Amount = budgetDto.Amount,
                StartDate = budgetDto.StartDate,  
                EndDate = budgetDto.EndDate,
                UserId = budgetDto.UserId,
                CategoryId = budgetDto.CategoryId,
            };
            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBudget), new {id = budget.BudgetId},budget);
        }

        private bool BudgetExists(int id)
        {
            return _context.Budgets.Any(e => e.BudgetId == id);
        }

        //PUT: api/Budget/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBudget(int id,[FromBody] BudgetDto budgetDto)
        {
            var budget = await _context.Budgets.FindAsync(id);
            if (budget == null)
            {
                return NotFound();
            }
            budget.Amount = budgetDto.Amount;
            budget.StartDate = budgetDto.StartDate;  
            budget.EndDate = budgetDto.EndDate;
            budget.UserId = budgetDto.UserId;
            budget.CategoryId = budgetDto.CategoryId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!BudgetExists(id))
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

        //DELETE: api/Budget/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(int id)
        {
            var budget = await _context.Budgets.FindAsync(id);
            if (budget == null)
            {
                return NotFound();
            }

            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}