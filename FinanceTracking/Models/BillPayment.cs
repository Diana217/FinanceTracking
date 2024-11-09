namespace FinanceTracking.Models;

public class BillPayment
{
    public int Id { get; set; }
    public string Description { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public Repeat Repeat { get; set; }
}
