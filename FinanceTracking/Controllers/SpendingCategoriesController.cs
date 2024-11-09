using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceTracking.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FinanceTracking.DTO;

namespace FinanceTracking.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SpendingCategoriesController : ControllerBase
{
    private readonly ApplicationContext _context;

    public SpendingCategoriesController(ApplicationContext context)
    {
        _context = context;
    }

    // GET: api/SpendingCategory
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SpendingCategory>>> GetSpendingCategories()
    {
        var userId = User.FindFirstValue("UserID");
        if (userId == null)
            return Unauthorized();

        //var categories = await _context.SpendingCategories
        //    .Where(category => category.UserId == userId)
        //    .ToListAsync();

        return null;
    }

    // GET: api/SpendingCategory/5
    [HttpGet("{id}")]
    public async Task<ActionResult<SpendingCategory>> GetSpendingCategory(int id)
    {
        var userId = User.FindFirstValue("UserID");
        if (userId == null)
            return Unauthorized();

        var spendingCategory = await _context.SpendingCategories.AsNoTracking()
            .FirstOrDefaultAsync(model => model.Id == id && model.UserId == userId);

        if (spendingCategory == null)
        {
            return NotFound();
        }

        return spendingCategory;
    }

    // PUT: api/SpendingCategory/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSpendingCategory(int id, SpendingCategoryModel spendingCategory)
    {
        var userId = User.FindFirstValue("UserID");
        if (userId == null)
            return Unauthorized();

        var existingCategory = await _context.SpendingCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(model => model.Id == id && model.UserId == userId);

        if (existingCategory == null)
        {
            return NotFound();
        }

        existingCategory.Name = spendingCategory.Name;

        _context.Entry(existingCategory).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/SpendingCategory
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<SpendingCategory>> CreateSpendingCategory(SpendingCategoryModel category)
    {
        var userId = User.FindFirstValue("UserID");
        if (userId == null)
            return Unauthorized();

        SpendingCategory spendingCategory = new SpendingCategory();
        spendingCategory.Name = category.Name;
        spendingCategory.UserId = userId;

        _context.SpendingCategories.Add(spendingCategory);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetSpendingCategory", new { id = spendingCategory.Id }, spendingCategory);
    }

    // DELETE: api/SpendingCategory/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSpendingCategory(int id)
    {
        var spendingCategory = await _context.SpendingCategories.FindAsync(id);
        if (spendingCategory == null)
        {
            return NotFound();
        }

        _context.SpendingCategories.Remove(spendingCategory);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
