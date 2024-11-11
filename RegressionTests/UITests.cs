using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace RegressionTests;

public class Tests
{
    private IWebDriver _driver;
    private WebDriverWait _wait;

    [SetUp]
    public void Setup()
    {
        _driver = new ChromeDriver();
        _driver.Manage().Window.Maximize();
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
    }

    [Test]
    public void OpenDashboardPage()
    {
        // Navigate to the login page
        _driver.Navigate().GoToUrl("https://localhost:5173/login");

        // Enter credentials and click the login button
        _driver.FindElement(By.Id("username")).SendKeys("Diana");
        _driver.FindElement(By.Id("password")).SendKeys("Finance_543!");
        _driver.FindElement(By.CssSelector("button.button")).Click();

        // Wait for the progress bar's title to be present
        try
        {
            // Adjust the selector to target the title attribute of the progress bar
            _wait.Until(driver => driver.FindElement(By.CssSelector(".goal-progress .progress-bar")).GetAttribute("title"));

            // Check if the title contains the correct information
            string progressTitle = _driver.FindElement(By.CssSelector(".goal-progress .progress-bar")).GetAttribute("title");
            Assert.IsTrue(progressTitle.Contains("% reached"), "Title attribute not found or does not contain expected text.");
        }
        catch (WebDriverTimeoutException)
        {
            Assert.Fail("Progress bar or title attribute not found after login.");
        }
    }

    [Test]
    public void NavigateToExpensesPage()
    {
        // Navigate to the login page
        _driver.Navigate().GoToUrl("https://localhost:5173/login");

        // Enter credentials and click the login button
        _driver.FindElement(By.Id("username")).SendKeys("Diana");
        _driver.FindElement(By.Id("password")).SendKeys("Finance_543!");
        _driver.FindElement(By.CssSelector("button.button")).Click();

        // Wait for the dashboard page to load and a link/button to the expenses page to appear
        try
        {
            _wait.Until(driver => driver.FindElement(By.CssSelector("a[href='/expenses']")));

            // Click the "Expenses" link/button
            _driver.FindElement(By.CssSelector("a[href='/expenses']")).Click();

            // Wait for the expenses page to load and check the presence of the expenses table
            _wait.Until(driver => driver.FindElement(By.CssSelector("table")));

            // Assert that the expenses table is displayed
            Assert.IsTrue(_driver.FindElement(By.CssSelector("table")).Displayed, "Expenses table is not displayed after navigation.");
        }
        catch (WebDriverTimeoutException)
        {
            Assert.Fail("Failed to navigate to the Expenses page.");
        }
    }

    [Test]
    public void AddNewExpense()
    {
        // Navigate to the login page
        _driver.Navigate().GoToUrl("https://localhost:5173/login");

        // Enter credentials and click the login button
        _driver.FindElement(By.Id("username")).SendKeys("Diana");
        _driver.FindElement(By.Id("password")).SendKeys("Finance_543!");
        _driver.FindElement(By.CssSelector("button.button")).Click();

        // Wait for the "Expenses" link to be visible (on the dashboard)
        try
        {
            _wait.Until(driver => driver.FindElement(By.CssSelector("a[href='/expenses']")));
            // Click the "Expenses" link/button to go to the expenses list page
            _driver.FindElement(By.CssSelector("a[href='/expenses']")).Click();
        }
        catch (WebDriverTimeoutException)
        {
            Assert.Fail("Failed to find 'Expenses' link after login.");
        }

        // Wait for the "/expenses" page to fully load and check for the "Add Expense" link
        try
        {
            _wait.Until(driver => driver.FindElement(By.CssSelector("a[href='/create-expense']")));
            // Navigate to the "Add Expense" page
            _driver.FindElement(By.CssSelector("a[href='/create-expense']")).Click();
        }
        catch (WebDriverTimeoutException)
        {
            Assert.Fail("Failed to find 'Create Expense' link on the Expenses page.");
        }

        // Wait for the expense form to load
        try
        {
            _wait.Until(driver => driver.FindElement(By.CssSelector(".create-form")));
        }
        catch (WebDriverTimeoutException)
        {
            Assert.Fail("Expense form did not load.");
        }

        // Fill out the form fields for a new expense
        _driver.FindElement(By.Id("category")).SendKeys("Food");
        _driver.FindElement(By.Id("amount")).SendKeys("100");
        _driver.FindElement(By.Id("date")).SendKeys("11-11-2024");

        // Submit the form
        _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        // Wait for the page to load again and check if the new expense appears in the table
        try
        {
            _wait.Until(driver => driver.FindElement(By.CssSelector("table tbody tr")));
            // Verify that the newly added expense is displayed in the table
            string newExpenseText = _driver.FindElement(By.CssSelector("table tbody tr:last-child td:nth-child(3)")).Text;
            Assert.AreEqual("100", newExpenseText, "The new expense amount is not correctly displayed.");
        }
        catch (WebDriverTimeoutException)
        {
            Assert.Fail("Failed to add the new expense or the expense did not appear in the table.");
        }
    }

    [TearDown]
    public void Teardown()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}