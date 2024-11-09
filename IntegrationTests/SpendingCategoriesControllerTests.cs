using System.Net;
using System.Net.Http.Json;
using FinanceTracking.Models;
using FinanceTracking.DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using FinanceTracking.Services;
using Microsoft.AspNetCore.Identity;

namespace IntegrationTests;

public class Tests
{
    [TestFixture]
    public class SpendingCategoriesControllerTests
    {
        private HttpClient _client;
        private WebApplicationFactory<Program> _factory;

        [SetUp]
        public void SetUp()
        {
            _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<ApplicationContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDatabase");
                    });
                });
            });

            _client = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        // Helper method to seed the user in the in-memory database
        private async Task SeedUserAsync(string userId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var user = await userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    user = new User { Id = userId, UserName = "testUser" };
                    await userManager.CreateAsync(user, "Test@1234");
                }
                else
                {
                    // If the user is found but causes tracking conflicts, detach it
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                    context.Entry(user).State = EntityState.Detached;
                }
            }
        }

        // Helper method to generate JWT token for a user
        private string GenerateJwtTokenForUser(string userId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var user = userManager.FindByIdAsync(userId).Result;

                if (user == null)
                    throw new InvalidOperationException("User not found.");

                var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
                return tokenService.CreateToken(user);
            }
        }

        [Test]
        public async Task GetSpendingCategories_ReturnsUnauthorized_WhenUserNotLoggedIn()
        {
            var response = await _client.GetAsync("/api/SpendingCategories");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task CreateSpendingCategory_ReturnsCreated_WhenValidData()
        {
            // Arrange
            var category = new SpendingCategoryModel { Name = "Food" };
            var userId = "testUser";

            // Seed the user into the in-memory database for the test
            await SeedUserAsync(userId);

            // Generate a JWT token for the user
            var token = GenerateJwtTokenForUser(userId);

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            // Act
            var response = await _client.PostAsJsonAsync("/api/SpendingCategories", category);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var createdCategory = await response.Content.ReadFromJsonAsync<SpendingCategory>();
            Assert.That(createdCategory?.Name, Is.EqualTo("Food"));
            Assert.That(createdCategory?.UserId, Is.EqualTo(userId));
        }

        [Test]
        public async Task GetSpendingCategory_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            var userId = "testUser";

            // Seed the user and generate a JWT token
            await SeedUserAsync(userId);
            var token = GenerateJwtTokenForUser(userId);

            // Add the JWT token to the request headers
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            // Act
            var response = await _client.GetAsync("/api/SpendingCategories/999");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }


        [Test]
        public async Task UpdateSpendingCategory_ReturnsNoContent_WhenCategoryUpdated()
        {
            var userId = "testUser";

            // Seed the user and generate a JWT token
            await SeedUserAsync(userId);
            var token = GenerateJwtTokenForUser(userId);

            // Add the JWT token to the request headers
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            // Seed the category in the in-memory database
            var category = new SpendingCategory { Name = "Entertainment", UserId = userId };
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                context.SpendingCategories.Add(category);
                await context.SaveChangesAsync();
            }

            // Prepare the updated category
            var updatedCategory = new SpendingCategoryModel { Name = "Leisure" };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/SpendingCategories/{category.Id}", updatedCategory);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

            // Verify the category was updated
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                var result = await context.SpendingCategories.FindAsync(category.Id);
                Assert.That(result?.Name, Is.EqualTo("Leisure"));
            }
        }

        [Test]
        public async Task DeleteSpendingCategory_ReturnsNoContent_WhenCategoryDeleted()
        {
            var userId = "testUser";

            // Seed the user and generate a JWT token
            await SeedUserAsync(userId);
            var token = GenerateJwtTokenForUser(userId);

            // Add the JWT token to the request headers
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            // Seed the category in the in-memory database
            var category = new SpendingCategory { Name = "Bills", UserId = userId };
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                context.SpendingCategories.Add(category);
                await context.SaveChangesAsync();
            }

            // Act
            var response = await _client.DeleteAsync($"/api/SpendingCategories/{category.Id}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

            // Verify the category was deleted
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                var deletedCategory = await context.SpendingCategories.FindAsync(category.Id);
                Assert.IsNull(deletedCategory);
            }
        }
    }
}