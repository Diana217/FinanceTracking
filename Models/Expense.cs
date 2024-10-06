using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceTracking.Models;

public class Expense
{
    public int Id { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    public int CategoryId { get; set; }
    public virtual SpendingCategory Category { get; set; } = null!;
    public DateTime Date { get; set; }
}
