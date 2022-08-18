using Demo.WebAPI.Models.Business;
using Demo.WebAPI.Models.Request;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Demo.WebAPI.Tests
{
    public class AddressController_IT
    {

        [Fact]
        public async Task Get_Returns_Data()
        {
            // Arrange
            var expectedAddress = new Address { Id = 1, Zip = "90210" };
            var webHost = new WebApplicationFactory<Program>();
            var client = webHost.CreateClient();

            // Act
            var response = await client.GetAsync($"api/address/{expectedAddress.Id}");            

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var actualAddress = await response.Content.ReadAsAsync<AddressRequest>();
            Assert.Equal(expectedAddress.Id, actualAddress.Id);
            Assert.Equal(expectedAddress.Zip, actualAddress.Zip);
        }

        [Fact]
        public async Task Post_Inserts_Data()
        {
            // Arrange
            var addressReq = new AddressRequest { Line1 = "L1 Post", City="Somewhere", State="KY", Zip = "22222" };
            var webHost = new WebApplicationFactory<Program>();
            var client = webHost.CreateClient();

            // Act
            var response = await client.PostAsJsonAsync("api/address", addressReq);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var actualAddress = await response.Content.ReadAsAsync<AddressRequest>();
            Assert.Equal(addressReq.Line1, actualAddress.Line1);
            Assert.Equal(addressReq.Zip, actualAddress.Zip);
        }

        [Fact]
        public async Task Put_Updates_Data()
        {
            // Arrange
            var addressReq = new AddressRequest { Id = 1, Line1 = "L1 PUT", Line2="L2 PUT", City = "Somewhere", State = "KY", Zip = "33333" };
            var webHost = new WebApplicationFactory<Program>();
            var client = webHost.CreateClient();

            // Act
            var response = await client.PutAsJsonAsync($"api/address/{addressReq.Id}", addressReq);
            
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var actualAddress = await response.Content.ReadAsAsync<AddressRequest>();
            Assert.Equal(addressReq.Line2, actualAddress.Line2);
            Assert.Equal(addressReq.Zip, actualAddress.Zip);
        }
    }
}
