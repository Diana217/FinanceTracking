using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceTracking.Models;
using System.Security.Claims;
using FinanceTracking.DTO;

namespace FinanceTracking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SavingsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public SavingsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Savings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Saving>>> GetSavings()
        {
            var userId = User.FindFirstValue("UserID");
            if (userId == null)
                return Unauthorized();

            var savings = await _context.Savings
                .Where(saving => saving.UserId == userId)
                .Include(saving => saving.Goal)
                .ToListAsync();

            return savings;
        }

        // GET: api/Savings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Saving>> GetSaving(int id)
        {
            var userId = User.FindFirstValue("UserID");
            if (userId == null)
                return Unauthorized();

            var saving = await _context.Savings.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id && model.UserId == userId);

            if (saving == null)
            {
                return NotFound();
            }

            return saving;
        }

        // PUT: api/Savings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSaving(int id, SavingModel saving)
        {
            var userId = User.FindFirstValue("UserID");
            if (userId == null)
                return Unauthorized();

            var existingSaving = await _context.Savings
                .AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id && model.UserId == userId);

            if (existingSaving == null)
            {
                return NotFound();
            }

            existingSaving.Amount = saving.Amount;
            existingSaving.GoalId = saving.GoalId;
            if (saving.Date != null)
            {
                existingSaving.Date = saving.Date.Value;
            }

            _context.Savings.Update(existingSaving);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Savings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Saving>> CreateSaving(SavingModel saving)
        {
            var userId = User.FindFirstValue("UserID");
            if (userId == null)
                return Unauthorized();

            Saving newSaving = new Saving();
            newSaving.Amount = saving.Amount;
            newSaving.GoalId = saving.GoalId;
            newSaving.Date = saving.Date ?? DateTime.Now;
            newSaving.UserId = userId;

            _context.Savings.Add(newSaving);
            await _context.SaveChangesAsync();

            var goal = await _context.FinancialGoals.FindAsync(newSaving.GoalId);

            if (goal != null && goal.IsCompleted != true)
            {
                var totalSavings = await _context.Savings.Where(goals => goals.GoalId == saving.GoalId).SumAsync(s => s.Amount);

                if (totalSavings >= goal.TargetAmount)
                {
                    goal.IsCompleted = true;
                    _context.FinancialGoals.Update(goal);
                    await _context.SaveChangesAsync();
                }
            }

            return CreatedAtAction("GetSaving", new { id = newSaving.Id }, newSaving);
        }

        // DELETE: api/Savings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSaving(int id)
        {
            var saving = await _context.Savings.FindAsync(id);
            if (saving == null)
            {
                return NotFound();
            }

            _context.Savings.Remove(saving);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
