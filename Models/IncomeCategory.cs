using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
namespace FinanceTracking.Models;

public class IncomeCategory
{
    public IncomeCategory()
    {
        Incomes = new HashSet<Income>();
    }
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Income> Incomes { get; set; }
}


public static class IncomeCategoryEndpoints
{
	public static void MapIncomeCategoryEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/IncomeCategory").WithTags(nameof(IncomeCategory));

        group.MapGet("/", async (ApplicationContext db) =>
        {
            return await db.IncomeCategories.ToListAsync();
        })
        .WithName("GetAllIncomeCategories")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<IncomeCategory>, NotFound>> (int id, ApplicationContext db) =>
        {
            return await db.IncomeCategories.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is IncomeCategory model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetIncomeCategoryById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, IncomeCategory incomeCategory, ApplicationContext db) =>
        {
            var affected = await db.IncomeCategories
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  //.SetProperty(m => m.Id, incomeCategory.Id)
                  .SetProperty(m => m.Name, incomeCategory.Name)
                  //.SetProperty(m => m.UserId, incomeCategory.UserId)
                  );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateIncomeCategory")
        .WithOpenApi();

        group.MapPost("/", async (IncomeCategory incomeCategory, ApplicationContext db) =>
        {
            db.IncomeCategories.Add(incomeCategory);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/IncomeCategory/{incomeCategory.Id}",incomeCategory);
        })
        .WithName("CreateIncomeCategory")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, ApplicationContext db) =>
        {
            var affected = await db.IncomeCategories
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteIncomeCategory")
        .WithOpenApi();
    }
}