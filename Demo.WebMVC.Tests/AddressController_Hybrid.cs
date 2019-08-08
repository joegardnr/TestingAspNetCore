using Demo.WebMVC.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Demo.WebMVC.Tests.TestHost
{
    public class AddressController_Hybrid
    {

        [Fact]
        public async Task Get_Returns_Data()
        {
            // Arrange
            var expectedAddress = new Address { Id = 1, Zip = "11111" };
            var repositoryMock = new MockAddressRepository();
            repositoryMock.GetAddressValue = expectedAddress;
            var webHost = new WebHostBuilder()
                .UseStartup<Demo.WebMVC.Startup>()
                .ConfigureTestServices(services => 
                {
                    services.AddSingleton<IAddressRepository>(repositoryMock);
                });
            var server = new TestServer(webHost);
            var client = server.CreateClient();

            // Act
            var response = await client.GetAsync($"address/{expectedAddress.Id}");
            var view = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("<input type=\"hidden\" id=\"id\" name=\"id\" value=\"1\" />", view);
            Assert.Contains("<input type=\"text\" class=\"form-control\" id=\"zip\" name=\"zip\" value=\"11111\" />", view);
            // There are way better libraries for doing the above, check out the "HTML Agility Pack"
        }
    }
}
