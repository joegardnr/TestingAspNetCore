using Demo.WebAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Demo.WebAPI.Tests.TestHost
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
                .UseStartup<Demo.WebAPI.Startup>()
                .ConfigureTestServices(services => 
                {
                    services.AddSingleton<IAddressRepository>(repositoryMock);
                });
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
            var repositoryMock = new MockAddressRepository();
            var webHost = new WebHostBuilder()
                .UseStartup<Demo.WebAPI.Startup>()
                .ConfigureTestServices(services =>
                {
                    services.AddSingleton<IAddressRepository>(repositoryMock);
                });
            var server = new TestServer(webHost);
            var client = server.CreateClient();

            // Act
            var response = await client.PostAsJsonAsync("api/address", expectedAddress);
            var actualAddress = await response.Content.ReadAsAsync<Address>();

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(expectedAddress.Line1, actualAddress.Line1);
            Assert.Equal(expectedAddress.Zip, actualAddress.Zip);
            Assert.Equal(expectedAddress.Line1, repositoryMock.InsertAddressValue.Line1);
        }

        [Fact]
        public async Task Put_Updates_Data()
        {
            // Arrange
            var expectedAddress = new Address { Id = 1, Line1="L1 PUT", Zip = "33333" };
            var repositoryMock = new MockAddressRepository();
            var webHost = new WebHostBuilder()
                .UseStartup<Demo.WebAPI.Startup>()
                .ConfigureTestServices(services =>
                {
                    services.AddSingleton<IAddressRepository>(repositoryMock);
                });
            var server = new TestServer(webHost);
            var client = server.CreateClient();

            // Act
            var response = await client.PutAsJsonAsync($"api/address/{expectedAddress.Id}", expectedAddress);
            var actualAddress = await response.Content.ReadAsAsync<Address>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedAddress.Zip, actualAddress.Zip);
            Assert.Equal(expectedAddress.Line1, repositoryMock.UpdateAddressValue.Line1);
            Assert.Equal(expectedAddress.Id, repositoryMock.UpdateAddressID);            
        }
    }

    /// <summary>
    /// You can use a Mocking library like Moq or NSubstitute, 
    /// but sometimes it's just as easy to roll your own code mocks.
    /// </summary>
    public class MockAddressRepository : IAddressRepository
    {
        public Address GetAddressValue;
        public Address InsertAddressValue;
        public int UpdateAddressID;
        public Address UpdateAddressValue;

        public Task<Address> GetAddressAsync(int id) => Task.FromResult(GetAddressValue);
        public Task<Address> InsertAddressAsync(Address address)
        {
            InsertAddressValue = address;
            return Task.FromResult(InsertAddressValue);
        }
        public Task<Address> UpdateAddressAsync(int id, Address address)
        {
            UpdateAddressID = id;
            UpdateAddressValue = address;
            return Task.FromResult(UpdateAddressValue);

        }
    }
}
