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
            var response = await controller.GetById(1) as OkObjectResult;
            var result = response.Value as Address;

            // Assert
            Assert.Same(expectedAddress, result);
        }

        [Fact]
        public async Task Post_Inserts_Data()
        {
            // Arrange   
            var expectedAddress = new Address {};
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
            var expectedId = 1;
            var expectedAddress = new Address { Id = expectedId };
            var repositoryMock = new MockAddressRepository();
            var controller = new AddressController(repositoryMock);            

            // Act
            var response = await controller.Put(expectedAddress.Id, expectedAddress) as OkObjectResult;
            var result = response.Value as Address;

            // Assert
            Assert.Same(expectedAddress, result);            
            Assert.Same(result, repositoryMock.UpdateAddressValue);
            Assert.Equal(expectedId, repositoryMock.UpdateAddressID);
        }

        [Fact]
        public async Task Put_Updates_Line3_Correctly()
        {
            // Arrange   
            var expectedLine3 = "Line 3 Put";
            var expectedAddress = new Address {Line3=expectedLine3 };
            var repositoryMock = new MockAddressRepository();
            var controller = new AddressController(repositoryMock);            

            // Act
            var response = await controller.Put(0, expectedAddress) as OkObjectResult;
            var result = response.Value as Address;

            // Assert
            Assert.Equal(expectedLine3, repositoryMock.UpdateAddressValue.Line3);
            Assert.Equal(expectedLine3, result.Line3);
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
