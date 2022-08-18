using Demo.WebAPI.Models.Business;
using Demo.WebAPI.Models.Request;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Demo.WebAPI.Tests
{
    public class SecureAddressController_IT
    {

        [Fact]
        public async Task Get_Returns_Data()
        {
            // Arrange
            var expectedAddressRequest = new AddressRequest { Id = 1, Zip = "90210" };
            var webHost = new WebApplicationFactory<Program>();
            var client = webHost.CreateClient();
            var jwt = GenerateJwt();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");            

            // Act
            var response = await client.GetAsync($"api/secureaddress/{expectedAddressRequest.Id}");          

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var actualAddress = await response.Content.ReadAsAsync<AddressRequest>();
            Assert.Equal(expectedAddressRequest.Id, actualAddress.Id);
            Assert.Equal(expectedAddressRequest.Zip, actualAddress.Zip);
        }

        [Fact]
        public async Task Get_Returns_401_Unauthorized()
        {
            // Arrange            
            var webHost = new WebApplicationFactory<Program>();
            var client = webHost.CreateClient();

            // Act
            var response = await client.GetAsync($"api/secureaddress/1");            

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Post_Inserts_Data()
        {
            // Arrange
            var expectedAddressRequest = new AddressRequest { Line1 = "L1 Post", City="Somewhere", State="KY", Zip = "22222" };
            var webHost = new WebApplicationFactory<Program>();
            var client = webHost.CreateClient();
            var jwt = GenerateJwt();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");

            // Act
            var response = await client.PostAsJsonAsync("api/secureaddress", expectedAddressRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var actualAddress = await response.Content.ReadAsAsync<Address>();
            Assert.Equal(expectedAddressRequest.Line1, actualAddress.Line1);
            Assert.Equal(expectedAddressRequest.Zip, actualAddress.Zip);
        }

        [Fact]
        public async Task Put_Updates_Data()
        {
            // Arrange
            var expectedAddressRequest = new AddressRequest { Id = 1, Line1="L1 PUT", Line2 = "L3 PUT", City = "Somewhere", State = "KY", Zip = "33333" };
            var webHost = new WebApplicationFactory<Program>();
            var client = webHost.CreateClient();
            var jwt = GenerateJwt();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");

            // Act
            var response = await client.PutAsJsonAsync($"api/secureaddress/{expectedAddressRequest.Id}", expectedAddressRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var actualAddress = await response.Content.ReadAsAsync<AddressRequest>();
            Assert.Equal(expectedAddressRequest.Line2, actualAddress.Line2);
            Assert.Equal(expectedAddressRequest.Zip, actualAddress.Zip);
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
