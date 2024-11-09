namespace FinanceTracking.DTO;

public class ExpenseModel
{
    public decimal Amount { get; set; }
    public int CategoryId { get; set; }
    public DateTime? Date { get; set; }
}
