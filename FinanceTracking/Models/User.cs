using Microsoft.AspNetCore.Identity;

namespace FinanceTracking.Models;

public class User : IdentityUser
{
}

public class UserValidator : IUserValidator<User>
{
    public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
    {
        List<IdentityError> errors = new List<IdentityError>();

        return Task.FromResult(errors.Count == 0 ?
            IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
    }
}

public class PasswordValidator : IPasswordValidator<User>
{
    public int RequiredLength { get; set; }

    public PasswordValidator(int length)
    {
        RequiredLength = length;
    }

    public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string password)
    {
        List<IdentityError> errors = new List<IdentityError>();

        if (String.IsNullOrEmpty(password) || password.Length < RequiredLength)
        {
            errors.Add(new IdentityError
            {
                Description = $"Мінімальна довжина пароля дорівнює {RequiredLength}!"
            });
        }

        return Task.FromResult(errors.Count == 0 ?
            IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
    }
}
