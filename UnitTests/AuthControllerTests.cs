using FinanceTracking.Controllers;
using FinanceTracking.Models;
using FinanceTracking.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.AspNetCore.Http;


namespace UnitTests;

[TestFixture]
public class AuthControllerTests
{
    private Mock<UserManager<User>> _userManagerMock;
    private Mock<SignInManager<User>> _signInManagerMock;
    private Mock<ITokenService> _tokenServiceMock;
    private AuthController _authController;

    [SetUp]
    public void SetUp()
    {
        _userManagerMock = MockUserManager<User>();
        _signInManagerMock = MockSignInManager<User>();
        _tokenServiceMock = new Mock<ITokenService>();

        _authController = new AuthController(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _tokenServiceMock.Object
        );
    }

    [Test]
    public async Task Register_ValidModel_ReturnsToken()
    {
        // Arrange
        var model = new RegisterModel { Username = "testuser", Email = "test@example.com", Password = "Password123!" };
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), "User"))
                        .ReturnsAsync(IdentityResult.Success);
        _tokenServiceMock.Setup(x => x.CreateToken(It.IsAny<User>())).Returns("test-token");

        // Act
        var result = await _authController.Register(model);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        var response = okResult?.Value as AuthResponse;
        Assert.That(response?.Token, Is.EqualTo("test-token"));
    }

    [Test]
    public async Task Register_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        _authController.ModelState.AddModelError("Error", "Invalid model");

        // Act
        var result = await _authController.Register(new RegisterModel());

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    [Test]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var model = new LoginModel { Username = "testuser", Password = "Password123!" };
        var user = new User { UserName = "testuser" };

        _userManagerMock.Setup(x => x.FindByNameAsync(model.Username)).ReturnsAsync(user);
        _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, model.Password, false))
                          .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
        _tokenServiceMock.Setup(x => x.CreateToken(user)).Returns("test-token");

        // Act
        var result = await _authController.Login(model);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        var response = okResult?.Value as AuthResponse;
        Assert.That(response?.Token, Is.EqualTo("test-token"));
    }

    [Test]
    public async Task Login_InvalidUsername_ReturnsUnauthorized()
    {
        // Arrange
        var model = new LoginModel { Username = "unknown", Password = "Password123!" };
        _userManagerMock.Setup(x => x.FindByNameAsync(model.Username)).ReturnsAsync((User)null);

        // Act
        var result = await _authController.Login(model);

        // Assert
        Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
        var unauthorizedResult = result as UnauthorizedObjectResult;
        Assert.That(unauthorizedResult?.Value, Is.EqualTo("Invalid username or password."));
    }

    [Test]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var model = new LoginModel { Username = "testuser", Password = "wrongpassword" };
        var user = new User { UserName = "testuser" };

        _userManagerMock.Setup(x => x.FindByNameAsync(model.Username)).ReturnsAsync(user);
        _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, model.Password, false))
                          .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        // Act
        var result = await _authController.Login(model);

        // Assert
        Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
        var unauthorizedResult = result as UnauthorizedObjectResult;
        Assert.That(unauthorizedResult?.Value, Is.EqualTo("Invalid username or password."));
    }

    private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
    }

    private static Mock<SignInManager<TUser>> MockSignInManager<TUser>() where TUser : class
    {
        var userManagerMock = MockUserManager<TUser>();
        return new Mock<SignInManager<TUser>>(
            userManagerMock.Object,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<TUser>>().Object,
            null, null, null, null
        );
    }
}
