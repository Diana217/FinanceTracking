using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceTracking.Models;
using System.Security.Claims;
using FinanceTracking.DTO;

namespace FinanceTracking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public ExpensesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Expenses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses()
        {
            var userId = User.FindFirstValue("UserID");
            if (userId == null)
                return Unauthorized();

            var expenses = await _context.Expenses
                .Where(expense => expense.UserId == userId)
                .Include(expense => expense.Category)
                .ToListAsync();

            return expenses;
        }

        // GET: api/Expenses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetExpense(int id)
        {
            var userId = User.FindFirstValue("UserID");
            if (userId == null)
                return Unauthorized();

            var expense = await _context.Expenses.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id && model.UserId == userId);

            if (expense == null)
            {
                return NotFound();
            }

            return expense;
        }

        // PUT: api/Expenses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, ExpenseModel expense)
        {
            var userId = User.FindFirstValue("UserID");
            if (userId == null)
                return Unauthorized();

            var existingExpense = await _context.Expenses
                .AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id && model.UserId == userId);

            if (existingExpense == null)
            {
                return NotFound();
            }

            existingExpense.Amount = expense.Amount;
            existingExpense.CategoryId = expense.CategoryId;
            if (expense.Date != null)
            {
                existingExpense.Date = expense.Date.Value;
            }

            _context.Expenses.Update(existingExpense);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Expenses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Expense>> PostExpense(ExpenseModel expense)
        {
            var userId = User.FindFirstValue("UserID");
            if (userId == null)
                return Unauthorized();

            Expense newExpense = new Expense();
            newExpense.Amount = expense.Amount;
            newExpense.CategoryId = expense.CategoryId;
            newExpense.Date = expense.Date ?? DateTime.Now;
            newExpense.UserId = userId;

            _context.Expenses.Add(newExpense);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExpense", new { id = newExpense.Id }, newExpense);
        }

        // DELETE: api/Expenses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
            {
                return NotFound();
            }

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
