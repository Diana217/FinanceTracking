using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceTracking.Models;
using System.Security.Claims;
using FinanceTracking.DTO;

namespace FinanceTracking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinancialGoalsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public FinancialGoalsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/FinancialGoals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FinancialGoal>>> GetFinancialGoals()
        {
            var userId = User.FindFirstValue("UserID");
            if (userId == null)
                return Unauthorized();

            var goals = await _context.FinancialGoals
                .Where(goal => goal.UserId == userId)
                .ToListAsync();

            return goals;
        }

        // GET: api/FinancialGoals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FinancialGoal>> GetFinancialGoal(int id)
        {
            var userId = User.FindFirstValue("UserID");
            if (userId == null)
                return Unauthorized();

            var goal = await _context.FinancialGoals.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id && model.UserId == userId);

            if (goal == null)
            {
                return NotFound();
            }

            return goal;
        }

        // PUT: api/FinancialGoals/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFinancialGoal(int id, FinancialGoalModel financialGoal)
        {
            var userId = User.FindFirstValue("UserID");
            if (userId == null)
                return Unauthorized();

            var existingGoal = await _context.FinancialGoals
                .AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id && model.UserId == userId);

            if (existingGoal == null)
            {
                return NotFound();
            }

            existingGoal.Name = financialGoal.Name;
            existingGoal.TargetAmount = financialGoal.TargetAmount;
            existingGoal.StartDate = financialGoal.StartDate;
            existingGoal.EndDate = financialGoal.EndDate;

            _context.Entry(existingGoal).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/FinancialGoals
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FinancialGoal>> CreateFinancialGoal(FinancialGoalModel financialGoal)
        {
            var userId = User.FindFirstValue("UserID");
            if (userId == null)
                return Unauthorized();

            FinancialGoal goal = new FinancialGoal();
            goal.Name = financialGoal.Name;
            goal.TargetAmount = financialGoal.TargetAmount;
            goal.StartDate = financialGoal.StartDate;
            goal.EndDate = financialGoal.EndDate;
            goal.UserId = userId;

            _context.FinancialGoals.Add(goal);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFinancialGoal", new { id = goal.Id }, goal);
        }

        // DELETE: api/FinancialGoals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFinancialGoal(int id)
        {
            var financialGoal = await _context.FinancialGoals.FindAsync(id);
            if (financialGoal == null)
            {
                return NotFound();
            }

            _context.FinancialGoals.Remove(financialGoal);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
