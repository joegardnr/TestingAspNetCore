using Demo.WebMVC.Controllers;
using Demo.WebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Demo.WebMVC.Tests
{
    public class AddressController_UT_Bad
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
            var response = await controller.Get(1) as ViewResult;
            var result = response.Model as Address;

            // Assert
            Assert.Same(expectedAddress, result);
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
