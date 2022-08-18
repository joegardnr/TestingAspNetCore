using Microsoft.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace Demo.BlazorServer.Tests
{
    public class Counter_IT : PageTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task AddressPageLoads()
        {
            // Act
            await Page.GotoAsync("http://localhost:5138/counter");

            // Assert
            await Expect(Page).ToHaveTitleAsync(new Regex("Counter"));
        }
    }
}