using Alba;
using Demo.WebAPI.Controllers;
using Demo.WebAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Demo.WebAPI.Tests.Alba
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

            using (var testObject = SystemUnderTest
                .ForStartup<Startup>(p =>
                    p.ConfigureTestServices(services =>
                        services.AddSingleton<IAddressRepository>(repositoryMock))))
            {
                // Act
                var actualAddress = await testObject.GetAsJson<Address>($"/api/address/{expectedAddress.Id}");
                
                // Assert
                Assert.Equal(expectedAddress.Id, actualAddress.Id);
                Assert.Equal(expectedAddress.Zip, actualAddress.Zip);
            }
        }

        [Fact]
        public async Task Post_Inserts_Data()
        {
            // Arrange
            var expectedAddress = new Address { Line1 = "L1 Post", Zip = "22222" };
            var repositoryMock = new MockAddressRepository();

            using (var testObject = SystemUnderTest
                .ForStartup<Startup>(p => 
                    p.ConfigureTestServices(services => 
                        services.AddSingleton<IAddressRepository>(repositoryMock))))
            {
                // Act & Assert
                var result = await testObject.Scenario(s =>
                {
                    s.Post.Json<Address>(expectedAddress).ToUrl("/api/address");
                    s.StatusCodeShouldBe(HttpStatusCode.Created);
                });

                var actualAddress = result.ResponseBody.ReadAsJson<Address>();

                Assert.Equal(expectedAddress.Line1, actualAddress.Line1);
                Assert.Equal(expectedAddress.Zip, actualAddress.Zip);
                Assert.Equal(expectedAddress.Line1, repositoryMock.InsertAddressValue.Line1);
            }
        }        

        [Fact]
        public async Task Put_Updates_Data()
        {
            // Arrange
            var expectedAddress = new Address { Id = 1, Line3 = "L3 PUT", Zip = "33333" };
            var repositoryMock = new MockAddressRepository();
            var webHost = new WebHostBuilder()
                .UseStartup<Demo.WebAPI.Startup>()
                .ConfigureTestServices(services =>
                {
                    services.AddSingleton<IAddressRepository>(repositoryMock);
                });
            
            using (var testObject = new SystemUnderTest(webHost))
            {
                // Act
                var actualAddress = await testObject
                    .PutJson<Address>(expectedAddress, $"/api/address/{expectedAddress.Id}")
                    .Receive<Address>();

                // Assert
                Assert.Equal(expectedAddress.Zip, actualAddress.Zip);
                Assert.Equal(expectedAddress.Line3, repositoryMock.UpdateAddressValue.Line3);
                Assert.Equal(expectedAddress.Id, repositoryMock.UpdateAddressID);
            }
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
