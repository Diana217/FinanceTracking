namespace FinanceTracking.DTO;

public class FinancialGoalModel
{
    public string Name { get; set; } = null!;
    public decimal TargetAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
