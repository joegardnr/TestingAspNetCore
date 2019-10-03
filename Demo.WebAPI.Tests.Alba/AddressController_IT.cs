using Alba;
using Demo.WebAPI.Controllers;
using Demo.WebAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Demo.WebAPI.Tests.Alba
{
    public class AddressController_IT
    {
        [Fact]
        public async Task Get_Returns_Data()
        {
            // Arrange
            var expectedAddress = new Address { Id = 1, Zip = "90210" };
            using(var testObject = SystemUnderTest.ForStartup<Startup>())
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
            using (var testObject = SystemUnderTest.ForStartup<Startup>())
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
            }
        }        

        [Fact]
        public async Task Put_Updates_Data()
        {
            // Arrange
            var expectedAddress = new Address { Id = 1, Line2="L2 PUT IT", Zip = "33333" };
            using (var testObject = SystemUnderTest.ForStartup<Startup>())
            {
                // Act
                var actualAddress = await testObject
                    .PutJson<Address>(expectedAddress, $"/api/address/{expectedAddress.Id}")
                    .Receive<Address>();

                // Assert
                Assert.Equal(expectedAddress.Line2, actualAddress.Line2);
                Assert.Equal(expectedAddress.Zip, actualAddress.Zip);
            }
        }

    }
}
