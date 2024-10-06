using Microsoft.AspNetCore.Identity;

namespace FinanceTracking.Models;

public class Role : IdentityRole
{
    public string Name { get; set; }
}
