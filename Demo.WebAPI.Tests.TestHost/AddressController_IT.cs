using Demo.WebAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Demo.WebAPI.Tests.TestHost
{
    public class AddressController_IT
    {

        [Fact]
        public async Task Get_Returns_Data()
        {
            // Arrange
            var expectedAddress = new Address { Id = 1, Zip = "90210" };
            var webHost = new WebHostBuilder().UseStartup<Demo.WebAPI.Startup>();
            var server = new TestServer(webHost);
            var client = server.CreateClient();

            // Act
            var response = await client.GetAsync($"api/address/{expectedAddress.Id}");
            var actualAddress = await response.Content.ReadAsAsync<Address>();            

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedAddress.Id, actualAddress.Id);
            Assert.Equal(expectedAddress.Zip, actualAddress.Zip);
        }

        [Fact]
        public async Task Post_Inserts_Data()
        {
            // Arrange
            var expectedAddress = new Address { Line1 = "L1 Post", Zip = "22222" };
            var webHost = new WebHostBuilder().UseStartup<Demo.WebAPI.Startup>();
            var server = new TestServer(webHost);
            var client = server.CreateClient();

            // Act
            var response = await client.PostAsJsonAsync("api/address", expectedAddress);
            var actualAddress = await response.Content.ReadAsAsync<Address>();

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(expectedAddress.Line1, actualAddress.Line1);
            Assert.Equal(expectedAddress.Zip, actualAddress.Zip);
        }

        [Fact]
        public async Task Put_Updates_Data()
        {
            // Arrange
            var expectedAddress = new Address { Id = 1, Line2="Line 3 PUT", Zip = "33333" };
            var webHost = new WebHostBuilder().UseStartup<Demo.WebAPI.Startup>();
            var server = new TestServer(webHost);
            var client = server.CreateClient();

            // Act
            var response = await client.PutAsJsonAsync($"api/address/{expectedAddress.Id}", expectedAddress);
            var actualAddress = await response.Content.ReadAsAsync<Address>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedAddress.Line2, actualAddress.Line2);
            Assert.Equal(expectedAddress.Zip, actualAddress.Zip);
        }
    }
}
