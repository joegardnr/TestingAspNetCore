using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace Demo.WebMVC.Tests.Playwright
{
    public class AddressController_IT : PageTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task AddressPageLoads()
        {
            // Act
            await Page.GotoAsync("http://localhost:5735/address/");

            // Assert
            await Expect(Page).ToHaveTitleAsync(new Regex("Address Page"));            
        }

        [Test]
        public async Task AddressPageHasSubmitButton()
        {
            // Act
            await Page.GotoAsync("http://localhost:5735/address/");

            // Assert            
            var submitButton = Page.Locator("text=Submit");
            await Expect(submitButton).ToHaveAttributeAsync("type", "submit");
        }

        [Test]
        public async Task SubmitAddressWorks()
        {
            // Arrange            
            await Page.GotoAsync("http://localhost:5735/address/");

            await Page.Locator("id=line1").FillAsync("1 Test Line");
            await Page.Locator("id=city").FillAsync("Test City");
            await Page.Locator("id=state").FillAsync("ST");
            await Page.Locator("id=zip").FillAsync("12345");


            // Assert            
            var submitButton = Page.Locator("text=Submit");
            await submitButton.ClickAsync();

            await Expect(Page).ToHaveURLAsync(new Regex("/address/[0-9]*"));
            await Expect(Page.Locator("id=line1")).ToHaveValueAsync("1 Test Line");
            await Expect(Page.Locator("id=city")).ToHaveValueAsync("Test City");
            await Expect(Page.Locator("id=state")).ToHaveValueAsync("ST");
            await Expect(Page.Locator("id=zip")).ToHaveValueAsync("12345");

            //await Page.ScreenshotAsync(new() { Path = "address_screenshot.png" });

        }
    }
}