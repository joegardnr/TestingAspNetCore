using Demo.WebMVC.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace Demo.WebMVC.Tests
{
    public class AddressController_IT
    {

        [Fact]
        public async Task Get_Returns_Data()
        {
            // Arrange
            var expectedAddress = new Address { Id = 1, Zip = "90210" };
            var webHost = new WebHostBuilder().UseStartup<Demo.WebMVC.Startup>();
            var server = new TestServer(webHost);
            var client = server.CreateClient();

            // Act
            var response = await client.GetAsync($"address/{expectedAddress.Id}");
            var view = await response.Content.ReadAsStringAsync();


            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);            
            Assert.Contains("<input type=\"hidden\" id=\"id\" name=\"id\" value=\"1\" />", view);
            Assert.Contains("<input type=\"text\" class=\"form-control\" id=\"zip\" name=\"zip\" value=\"90210\" />", view);
            // There are way better libraries for doing the above, check out the "HTML Agility Pack"
        }
    }
}
