using Demo.WebAPI.Controllers;
using Demo.WebAPI.Models.Business;
using Demo.WebAPI.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Demo.WebAPI.Tests
{
    /// <summary>
    /// You can write "Unit Tests" like this if you want, 
    /// but it's a lot of work and makes it hard to change/refactor later.
    /// </summary>
    public class AddressController_UT_Bad
    {

        [Fact]
        public async Task Get_Returns_Data()
        {
            // Arrange
            var address = new Address { Id = 1 };
            var expectedResponse = new AddressRequest { Id = address.Id };

            var validatorMock = new Mock<IValidator>();            
            var mapperMock = new Mock<IMapper>();
            var repositoryMock = new Mock<IAddressRepository>();
                        
            mapperMock.Setup(m => m.MapToRequest(address)).Returns(expectedResponse);
            repositoryMock.Setup(r => r.GetAddressAsync(address.Id)).ReturnsAsync(address);
            
            var controller = new AddressController(validatorMock.Object, mapperMock.Object, repositoryMock.Object);

            // Act
            var response = await controller.GetById(1) as OkObjectResult;
            var result = response.Value as AddressRequest;

            // Assert
            Assert.Same(expectedResponse, result);
            repositoryMock.Verify(r => r.GetAddressAsync(address.Id), Times.Once);
        }

        [Fact]
        public async Task Post_Inserts_Data()
        {
            // Arrange
            var address = new Address { Id = 1 };
            var request = new AddressRequest { Id = address.Id };
            var expectedResponse = new AddressRequest { Id = address.Id };

            var validatorMock = new Mock<IValidator>();            
            var mapperMock = new Mock<IMapper>();
            var repositoryMock = new Mock<IAddressRepository>();

            validatorMock.Setup(v => v.Validate(request)).Returns(new List<string>());
            mapperMock.Setup(m => m.MapFromRequest(request)).Returns(address);
            repositoryMock.Setup(r => r.InsertAddressAsync(address)).ReturnsAsync(address);
            mapperMock.Setup(m => m.MapToRequest(address)).Returns(expectedResponse);
            
            var controller = new AddressController(validatorMock.Object, mapperMock.Object, repositoryMock.Object);

            // Act
            var result = await controller.Post(request) as CreatedAtActionResult;
            var actualResponse = result.Value as AddressRequest;

            // Assert            
            repositoryMock.Verify(r => r.InsertAddressAsync(address), Times.Once);
            Assert.Same(expectedResponse, actualResponse);
        }

        [Fact]
        public async Task Put_Updates_Data()
        {
            // Arrange
            var address = new Address { Id = 1 };
            var request = new AddressRequest() { Line2 = "Line 2 Put" };
            var expectedResponse = new AddressRequest { Id = address.Id };

            var validatorMock = new Mock<IValidator>();
            var mapperMock = new Mock<IMapper>();
            var repositoryMock = new Mock<IAddressRepository>();

            validatorMock.Setup(v => v.Validate(request)).Returns(new List<string>());
            mapperMock.Setup(m => m.MapFromRequest(request)).Returns(address);
            repositoryMock.Setup(r => r.UpdateAddressAsync(address.Id, address)).ReturnsAsync(address);
            mapperMock.Setup(m => m.MapToRequest(address)).Returns(expectedResponse);

            var controller = new AddressController(validatorMock.Object, mapperMock.Object, repositoryMock.Object);

            // Act
            var result = await controller.Put(address.Id, request) as OkObjectResult;
            var actualResponse = result.Value as AddressRequest;

            // Assert            
            repositoryMock.Verify(r => r.UpdateAddressAsync(address.Id, address), Times.Once);
            Assert.Same(expectedResponse, actualResponse);
        }
    }
}
