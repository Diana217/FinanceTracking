using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceTracking.Models;

public class Budget
{
    public int Id { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal Limit { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
