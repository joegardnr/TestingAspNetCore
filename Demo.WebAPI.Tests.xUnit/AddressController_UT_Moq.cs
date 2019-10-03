using Demo.WebAPI.Controllers;
using Demo.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Demo.WebAPI.Tests.xUnit
{
    public class AddressController_UT_Moq
    {
        private Address addressModel = new Address
        {
            Id = 1
        };

        [Fact]
        public async Task Get_Returns_Data()
        {
            // Arrange           
            var repositoryMock = new Mock<IAddressRepository>();
            repositoryMock.Setup(r => r.GetAddressAsync(addressModel.Id)).ReturnsAsync(addressModel);
            var controller = new AddressController(repositoryMock.Object);

            // Act
            var response = await controller.GetById(1) as OkObjectResult;
            var result = response.Value as Address;

            // Assert
            Assert.Same(addressModel, result);
            repositoryMock.Verify(r => r.GetAddressAsync(addressModel.Id), Times.Once);
        }

        [Fact]
        public async Task Post_Inserts_Data()
        {
            // Arrange           
            var repositoryMock = new Mock<IAddressRepository>();
            repositoryMock.Setup(r => r.InsertAddressAsync(addressModel)).ReturnsAsync(addressModel);
            var controller = new AddressController(repositoryMock.Object);

            // Act
            var response = await controller.Post(addressModel) as CreatedAtActionResult;
            var result = response.Value as Address;

            // Assert
            Assert.Same(addressModel, result);
            repositoryMock.Verify(r => r.InsertAddressAsync(addressModel), Times.Once);
        }

        [Fact]
        public async Task Put_Updates_Data()
        {
            // Arrange           
            var expectedLine2 = "Line 2 Put";
            var updatedAddress = new Address() { Line2 = expectedLine2};
            var repositoryMock = new Mock<IAddressRepository>();
            repositoryMock.Setup(r => r.UpdateAddressAsync(addressModel.Id, updatedAddress)).ReturnsAsync(updatedAddress);
            var controller = new AddressController(repositoryMock.Object);            

            // Act
            var response = await controller.Put(addressModel.Id, updatedAddress) as OkObjectResult;
            var result = response.Value as Address;

            // Assert
            Assert.Same(updatedAddress, result);
            repositoryMock.Verify(r => r.UpdateAddressAsync(addressModel.Id, updatedAddress), Times.Once);
            Assert.Equal(expectedLine2, updatedAddress.Line2);
        }
    }
}
