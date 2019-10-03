using Demo.WebAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Demo.WebAPI.Tests.TestHost
{
    public class SecureAddressController_IT
    {

        [Fact]
        public async Task Get_Returns_Data()
        {
            // Arrange
            var expectedAddress = new Address { Id = 1, Zip = "90210" };            
            var webHost = new WebHostBuilder().UseStartup<Demo.WebAPI.Startup>();
            var server = new TestServer(webHost);
            var client = server.CreateClient();
            var jwt = GenerateJwt();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");            

            // Act
            var response = await client.GetAsync($"api/secureaddress/{expectedAddress.Id}");
            var actualAddress = await response.Content.ReadAsAsync<Address>();            

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedAddress.Id, actualAddress.Id);
            Assert.Equal(expectedAddress.Zip, actualAddress.Zip);
        }

        [Fact]
        public async Task Get_Returns_401_Unauthorized()
        {
            // Arrange            
            var webHost = new WebHostBuilder().UseStartup<Demo.WebAPI.Startup>();
            var server = new TestServer(webHost);
            var client = server.CreateClient();            

            // Act
            var response = await client.GetAsync($"api/secureaddress/1");            

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Post_Inserts_Data()
        {
            // Arrange
            var expectedAddress = new Address { Line1 = "L1 Post", Zip = "22222" };
            var webHost = new WebHostBuilder().UseStartup<Demo.WebAPI.Startup>();
            var server = new TestServer(webHost);
            var client = server.CreateClient();
            var jwt = GenerateJwt();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");

            // Act
            var response = await client.PostAsJsonAsync("api/secureaddress", expectedAddress);
            var actualAddress = await response.Content.ReadAsAsync<Address>();

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(expectedAddress.Line1, actualAddress.Line1);
            Assert.Equal(expectedAddress.Zip, actualAddress.Zip);
        }

        [Fact]
        public async Task Put_Updates_Data()
        {
            // Arrange
            var expectedAddress = new Address { Id = 1, Line2 = "L3 PUT", Zip = "33333" };
            var webHost = new WebHostBuilder().UseStartup<Demo.WebAPI.Startup>();
            var server = new TestServer(webHost);
            var client = server.CreateClient();
            var jwt = GenerateJwt();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");

            // Act
            var response = await client.PutAsJsonAsync($"api/secureaddress/{expectedAddress.Id}", expectedAddress);
            var actualAddress = await response.Content.ReadAsAsync<Address>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedAddress.Line2, actualAddress.Line2);
            Assert.Equal(expectedAddress.Zip, actualAddress.Zip);
        }

        private string GenerateJwt()
        {
            var secret = Encoding.UTF8.GetBytes("a really super secret key that needs to be exactly 64 bytes long");
            var credentials = new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature);
            var customClaims = new Claim[] {                
                new Claim("sub", "user")
            };
            var token = new JwtSecurityToken(
               claims: customClaims,
               signingCredentials: credentials, expires: System.DateTime.Now.AddDays(1)
            );
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtString = jwtHandler.WriteToken(token);
            return jwtString;

        }
    }
}
