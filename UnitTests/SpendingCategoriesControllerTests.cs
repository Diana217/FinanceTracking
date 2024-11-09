using FinanceTracking.Controllers;
using FinanceTracking.DTO;
using FinanceTracking.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace UnitTests;

[TestFixture]
public class Tests
{
    private SpendingCategoriesController _spendingCategoriesController;
    private ApplicationContext _dbContext;

    [SetUp]
    public void SetUp()
    {
        var serviceCollection = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        serviceCollection.AddDbContext<ApplicationContext>(options =>
        {
            options.UseInMemoryDatabase("TestDB");
        });
        serviceCollection.AddSingleton<IConfiguration>(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        _dbContext = serviceProvider.GetRequiredService<ApplicationContext>();

        _spendingCategoriesController = new SpendingCategoriesController(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Test]
    public async Task GetAllAsync_ReturnsCorrectNumberOfSpendingCategories()
    {
        // Arrange
        var userId = "test-user-id";

        _spendingCategoriesController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("UserID", userId)
                }))
            }
        };

        await _dbContext.SpendingCategories.AddRangeAsync(
            new SpendingCategory { Name = "Spending category 1", UserId = userId },
            new SpendingCategory { Name = "Spending category 2", UserId = userId },
            new SpendingCategory { Name = "Spending category 3", UserId = userId }
        );
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _spendingCategoriesController.GetSpendingCategories();

        // Assert
        Assert.That(result.Value?.Count(), Is.EqualTo(3));
    }

    [Test]
    public async Task CreateAsync_AddsSpendingCategoryToDatabase()
    {
        // Arrange
        var userId = "test-user-id";

        _spendingCategoriesController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("UserID", userId)
                }))
            }
        };

        var category = new SpendingCategoryModel { Name = "Spending category 1" };

        // Act
        await _spendingCategoriesController.CreateSpendingCategory(category);

        // Assert
        var savedCategories = await _dbContext.SpendingCategories.Where(n => n.Name == "Spending category 1").ToListAsync();
        Assert.That(savedCategories.Count, Is.EqualTo(1));

        var savedCategory = savedCategories.FirstOrDefault();
        Assert.That(savedCategory?.Name, Is.EqualTo(category.Name));
    }

    [Test]
    public async Task CreateAsync_DoesNotAddSpendingCategoryWithoutName()
    {
        // Arrange
        var categoryWithoutName = new SpendingCategoryModel {  };

        // Act and Assert
        try
        {
            await _spendingCategoriesController.CreateSpendingCategory(categoryWithoutName);
        }
        catch (ArgumentException)
        {
            return;
        }

        Assert.Fail("Expected ArgumentException was not thrown.");
    }


    [Test]
    public async Task UpdateAsync_UpdatesSpendingCategoryInDatabase()
    {
        // Arrange
        var userId = "test-user-id";

        _spendingCategoriesController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("UserID", userId)
                }))
            }
        };

        var category = new SpendingCategory { Name = "Spending category 1", UserId = "test-user-id" };
        await _dbContext.SpendingCategories.AddAsync(category);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        var updatedCategory = new SpendingCategoryModel { Name = "Updated category 1" };

        // Act
        await _spendingCategoriesController.UpdateSpendingCategory(category.Id, updatedCategory);

        // Assert
        var savedCategory = await _spendingCategoriesController.GetSpendingCategory(category.Id);
        Assert.IsNotNull(savedCategory);
        Assert.That(savedCategory.Value?.Name, Is.EqualTo(updatedCategory.Name));
    }

    [Test]
    public async Task UpdateAsync_DoesNotUpdateNonexistentSpendingCategory()
    {
        // Arrange
        var userId = "test-user-id";

        _spendingCategoriesController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("UserID", userId)
                }))
            }
        };

        var category = new SpendingCategoryModel { Name = "Spending category 999" };

        // Act
        await _spendingCategoriesController.UpdateSpendingCategory(999, category);

        // Assert
        var savedCategory = await _dbContext.SpendingCategories.FirstOrDefaultAsync(n => n.Id == 999);
        Assert.IsNull(savedCategory);
    }
}