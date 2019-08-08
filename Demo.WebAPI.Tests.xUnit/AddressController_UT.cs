using Demo.WebAPI.Controllers;
using Demo.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

namespace Demo.WebAPI.Tests.xUnit
{
    public class AddressController_UT
    {
        [Fact]
        public async Task Get_Returns_Data()
        {
            // Arrange      
            var expectedAddress = new Address { Id = 1 };
            var repositoryMock = new MockAddressRepository();
            repositoryMock.GetAddressValue = expectedAddress;
            var controller = new AddressController(repositoryMock);

            // Act
            var response = await controller.Get(1) as OkObjectResult;
            var result = response.Value as Address;

            // Assert
            Assert.Same(expectedAddress, result);
        }

        [Fact]
        public async Task Post_Inserts_Data()
        {
            // Arrange   
            var expectedAddress = new Address { Id = 1 };
            var repositoryMock = new MockAddressRepository();            
            var controller = new AddressController(repositoryMock);

            // Act
            var response = await controller.Post(expectedAddress) as CreatedAtActionResult;
            var result = response.Value as Address;

            // Assert
            Assert.Same(expectedAddress, result);
            Assert.Same(result, repositoryMock.InsertAddressValue);
        }

        [Fact]
        public async Task Put_Updates_Data()
        {
            // Arrange   
            var expectedAddress = new Address { Id = 1 };
            var updatedAddress = new Address();
            var repositoryMock = new MockAddressRepository();
            var controller = new AddressController(repositoryMock);            

            // Act
            var response = await controller.Put(expectedAddress.Id, updatedAddress) as OkObjectResult;
            var result = response.Value as Address;

            // Assert
            Assert.Same(updatedAddress, result);            
            Assert.Same(result, repositoryMock.UpdateAddressValue);
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
