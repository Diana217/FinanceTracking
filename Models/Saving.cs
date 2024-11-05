using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceTracking.Models;

public class Saving
{
    public int Id { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    public int GoalId { get; set; }
    public virtual FinancialGoal Goal { get; set; } = null!;
    public DateTime Date { get; set; }
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
