using Demo.WebMVC.Models;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace Demo.WebMVC.Tests.Playwright
{
    /// <summary>
    /// This example uses a custom WebApplicationFactory to swap out the in-process TestHost/TestServer with a "real" Kestrel server.
    /// Since it uses the local network, it's not really a "unit" test, but is fairly self contained and arguably more reliable.
    /// Until MS makes this official, I wouldn't recommend it. (see Github issues referenced in CustomWebApplicationFactory).
    /// </summary>
    [TestFixture]
    [Ignore("Don't run these normally")]
    public class AddressController_IT_Local : PageTest
    {
        private string _serverAddress;
        private MockAddressRepository _addressRepository;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
           _addressRepository = new MockAddressRepository();
            var webHost = new CustomWebApplicationFactory(_addressRepository);                
            _serverAddress = webHost.ServerAddress.TrimEnd('/'); //makes later uses more readable
        }

        [Test]
        public async Task AddressPageLoads()
        {            
            // Act
            await Page.GotoAsync($"{_serverAddress}/address");

            // Assert
            await Expect(Page).ToHaveTitleAsync(new Regex("Address Page"));            
        }

        [Test]
        public async Task AddressPageHasSubmitButton()
        {
            // Act
            await Page.GotoAsync($"{_serverAddress}/address/");

            // Assert            
            var submitButton = Page.Locator("text=Submit");
            await Expect(submitButton).ToHaveAttributeAsync("type", "submit");
        }

        [Test]
        public async Task SubmitAddressWorks()
        {
            // Arrange
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

    /// <summary>
    /// You can use a Mocking library like Moq or NSubstitute, 
    /// but sometimes it's just as easy to roll your own code mocks.
    /// </summary>
    public class MockAddressRepository : IAddressRepository
    {
        private readonly IList<Address> _addressDB;  // A list is totally a legit "Database"...
        public MockAddressRepository()
        {
            _addressDB = new List<Address>();
        }

        public Task<Address> GetAddressAsync(int id)
        {
            var address = _addressDB.Single(a => a.Id == id);
            return Task.FromResult(address);
        }

        public Task<Address> InsertAddressAsync(Address address)
        {
            var maxId = _addressDB.Any() ? _addressDB.Max(r => r.Id) : 0;  // This is not very thread safe. 
            address.Id = maxId + 1;
            _addressDB.Add(address);
            return Task.FromResult(address);
        }

        public Task<Address> UpdateAddressAsync(int id, Address address)
        {
            var existing = _addressDB.Single(a => a.Id == id);
            _addressDB.Remove(existing);
            _addressDB.Add(address);
            return Task.FromResult(address);
        }
    }
}