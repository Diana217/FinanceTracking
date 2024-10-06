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
