using Demo.WebAPI.Models.Business;
using Demo.WebAPI.Models.Request;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Demo.WebAPI.Tests
{
    public class AddressController_UT
    {

        [Fact]
        public async Task Get_Returns_Data()
        {
            // Arrange
            var expectedAddress = new Address { Id = 1, Zip = "11111" };

            var repositoryMock = new MockAddressRepository();
            repositoryMock.GetAddressValue = expectedAddress;

            var webHost = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services => { 
                        services.AddSingleton<IAddressRepository>(repositoryMock);
                    });
                });
            var client = webHost.CreateClient();

            // Act
            var response = await client.GetAsync($"api/address/{expectedAddress.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var addressResponse = await response.Content.ReadAsAsync<AddressRequest>();
            Assert.Equal(expectedAddress.Id, addressResponse.Id);
            Assert.Equal(expectedAddress.Zip, addressResponse.Zip);
        }

        [Fact]
        public async Task Post_Inserts_Data()
        {
            // Arrange
            var addressRequest = new AddressRequest { Line1 = "L1 Post", City="Somewhere", State="KY", Zip = "22222" };
            var repositoryMock = new MockAddressRepository();

            var webHost = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services => {
                        services.AddSingleton<IAddressRepository>(repositoryMock);
                    });
                });
            var client = webHost.CreateClient();

            // Act
            var response = await client.PostAsJsonAsync("api/address", addressRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var addressInserted = repositoryMock.InsertAddressValue;  // This is the Address that was actually sent to the "Database"
            Assert.Equal(addressRequest.Line1, addressInserted.Line1);
            Assert.Equal(addressRequest.Zip, addressInserted.Zip);

            var addressResponse = await response.Content.ReadAsAsync<AddressRequest>();  // This is what came back from the "Real" http request
            Assert.Equal(addressRequest.Line1, addressResponse.Line1);
            Assert.Equal(addressRequest.Zip, addressResponse.Zip);
        }

        [Fact]
        public async Task Put_Updates_Data()
        {
            // Arrange
            var addressRequest = new AddressRequest { Id = 1, Line1="L1 PUT", Line2 = "L3 PUT", City = "Somewhere", State = "KY", Zip = "33333" };
            var repositoryMock = new MockAddressRepository();

            var webHost = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services => {
                        services.AddSingleton<IAddressRepository>(repositoryMock);
                    });
                });
            var client = webHost.CreateClient();

            // Act
            var response = await client.PutAsJsonAsync($"api/address/{addressRequest.Id}", addressRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var addressUpdated = repositoryMock.UpdateAddressValue;  // This is the Address that was actually sent to the "Database"
            Assert.Equal(addressRequest.Line2, addressUpdated.Line2);
            Assert.Equal(addressRequest.Zip, addressUpdated.Zip);

            var addressResponse = await response.Content.ReadAsAsync<AddressRequest>();
            Assert.Equal(addressRequest.Line2, addressResponse.Line2);
            Assert.Equal(addressRequest.Zip, addressResponse.Zip);
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
