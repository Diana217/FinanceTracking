using FinanceTracking.DTO;
using FinanceTracking.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinanceTracking.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class IncomeCategoriesController : ControllerBase
{
    private readonly ApplicationContext _context;

    public IncomeCategoriesController(ApplicationContext context)
    {
        _context = context;
    }

    // GET: api/IncomeCategory
    [HttpGet]
    public async Task<ActionResult<IEnumerable<IncomeCategory>>> GetIncomeCategories()
    {
        var userId = User.FindFirstValue("UserID");
        if (userId == null)
            return Unauthorized();

        var incomeCategories = await _context.IncomeCategories
            .Where(category => category.UserId == userId)
            .ToListAsync();

        return incomeCategories;
    }

    // GET: api/IncomeCategory/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<IncomeCategory>> GetIncomeCategory(int id)
    {
        var userId = User.FindFirstValue("UserID");
        if (userId == null)
            return Unauthorized();

        var incomeCategory = await _context.IncomeCategories.AsNoTracking()
            .FirstOrDefaultAsync(model => model.Id == id && model.UserId == userId);

        if (incomeCategory == null)
        {
            return NotFound();
        }

        return incomeCategory;
    }

    // PUT: api/IncomeCategory/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIncomeCategory(int id, IncomeCategoryModel incomeCategory)
    {
        var userId = User.FindFirstValue("UserID");
        if (userId == null)
            return Unauthorized();

        var existingCategory = await _context.IncomeCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(model => model.Id == id && model.UserId == userId);

        if (existingCategory == null)
        {
            return NotFound();
        }

        existingCategory.Name = incomeCategory.Name; 

        _context.IncomeCategories.Update(existingCategory);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/IncomeCategory
    [HttpPost]
    public async Task<ActionResult<IncomeCategory>> CreateIncomeCategory(IncomeCategoryModel category)
    {
        var userId = User.FindFirstValue("UserID");
        if (userId == null)
            return Unauthorized();

        IncomeCategory incomeCategory = new IncomeCategory();
        incomeCategory.Name = category.Name;
        incomeCategory.UserId = userId;

        _context.IncomeCategories.Add(incomeCategory);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetIncomeCategory), new { id = incomeCategory.Id }, incomeCategory);
    }

    // DELETE: api/IncomeCategory/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIncomeCategory(int id)
    {
        var incomeCategory = await _context.IncomeCategories.FindAsync(id);
        if (incomeCategory == null)
        {
            return NotFound();
        }

        _context.IncomeCategories.Remove(incomeCategory);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
