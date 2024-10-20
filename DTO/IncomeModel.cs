namespace FinanceTracking.DTO;

public class IncomeModel
{
    public decimal Amount { get; set; }
    public int CategoryId { get; set; }
    public DateTime? Date { get; set; }
}
