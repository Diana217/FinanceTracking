using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracking.Models;

public class ApplicationContext : IdentityDbContext<User>
{
    public ApplicationContext()
    {
    }

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options) { }

    public virtual DbSet<BillPayment> BillPayments { get; set; } = null!;
    public virtual DbSet<Budget> Budgets { get; set; } = null!;
    public virtual DbSet<Expense> Expenses { get; set; } = null!;
    public virtual DbSet<FinancialGoal> FinancialGoals { get; set; } = null!;
    public virtual DbSet<Income> Incomes { get; set; } = null!;
    public virtual DbSet<IncomeCategory> IncomeCategories { get; set; } = null!;
    public virtual DbSet<Saving> Savings { get; set; } = null!;
    public virtual DbSet<SpendingCategory> SpendingCategories { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-IDFUNTA\\SQLEXPRESS;Database=FinanceDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;");
        }
    }
}
