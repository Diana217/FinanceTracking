using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceTracking.Models;
using System.Security.Claims;
using FinanceTracking.DTO;

namespace FinanceTracking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncomesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public IncomesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Incomes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Income>>> GetIncomes()
        {
            var userId = User.FindFirstValue("UserID");
            if (userId == null)
                return Unauthorized();

            var incomes = await _context.Incomes
                .Where(income => income.UserId == userId)
                .Include(income => income.Category)
                .ToListAsync();

            return incomes;
        }

        // GET: api/Incomes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Income>> GetIncome(int id)
        {
            var userId = User.FindFirstValue("UserID");
            if (userId == null)
                return Unauthorized();

            var income = await _context.Incomes.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id && model.UserId == userId);

            if (income == null)
            {
                return NotFound();
            }

            return income;
        }

        // PUT: api/Incomes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIncome(int id, IncomeModel income)
        {
            var userId = User.FindFirstValue("UserID");
            if (userId == null)
                return Unauthorized();

            var existingIncome = await _context.Incomes
                .AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id && model.UserId == userId);

            if (existingIncome == null)
            {
                return NotFound();
            }

            existingIncome.Amount = income.Amount;
            existingIncome.CategoryId = income.CategoryId;
            if (income.Date != null)
            {
                existingIncome.Date = income.Date.Value;
            }

            _context.Incomes.Update(existingIncome);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Incomes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Income>> CreateIncome(IncomeModel income)
        {
            var userId = User.FindFirstValue("UserID");
            if (userId == null)
                return Unauthorized();

            Income newIncome = new Income();
            newIncome.Amount = income.Amount;
            newIncome.CategoryId = income.CategoryId;
            newIncome.Date = income.Date ?? DateTime.Now;
            newIncome.UserId = userId;

            _context.Incomes.Add(newIncome);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetIncome", new { id = newIncome.Id }, newIncome);
        }

        // DELETE: api/Incomes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncome(int id)
        {
            var income = await _context.Incomes.FindAsync(id);
            if (income == null)
            {
                return NotFound();
            }

            _context.Incomes.Remove(income);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
