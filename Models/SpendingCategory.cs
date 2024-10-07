using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
namespace FinanceTracking.Models;

public class SpendingCategory
{
    public SpendingCategory()
    {
        Expenses = new HashSet<Expense>();
    }
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Expense> Expenses { get; set; }
}


public static class SpendingCategoryEndpoints
{
	public static void MapSpendingCategoryEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/SpendingCategory").WithTags(nameof(SpendingCategory));

        group.MapGet("/", async (ApplicationContext db) =>
        {
            return await db.SpendingCategories.ToListAsync();
        })
        .WithName("GetAllSpendingCategories")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<SpendingCategory>, NotFound>> (int id, ApplicationContext db) =>
        {
            return await db.SpendingCategories.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is SpendingCategory model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetSpendingCategoryById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, SpendingCategory spendingCategory, ApplicationContext db) =>
        {
            var affected = await db.SpendingCategories
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  //.SetProperty(m => m.Id, spendingCategory.Id)
                  .SetProperty(m => m.Name, spendingCategory.Name)
                  //.SetProperty(m => m.UserId, spendingCategory.UserId)
                  );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateSpendingCategory")
        .WithOpenApi();

        group.MapPost("/", async (SpendingCategory spendingCategory, ApplicationContext db) =>
        {
            db.SpendingCategories.Add(spendingCategory);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/SpendingCategory/{spendingCategory.Id}",spendingCategory);
        })
        .WithName("CreateSpendingCategory")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, ApplicationContext db) =>
        {
            var affected = await db.SpendingCategories
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteSpendingCategory")
        .WithOpenApi();
    }
}