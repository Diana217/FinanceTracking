using FinanceTracking.Models;

namespace FinanceTracking.Services;

public interface ITokenService
{
    string CreateToken(User user);
}
