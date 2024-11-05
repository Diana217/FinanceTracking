using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceTracking.Models;

public class FinancialGoal
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    [Column(TypeName = "decimal(18,2)")]
    public decimal TargetAmount { get; set; }
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool? IsCompleted { get; set; }
}
