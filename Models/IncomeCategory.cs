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
