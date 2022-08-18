using Demo.WebAPI.Models.Business;
using Demo.WebAPI.Models.Request;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
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
    public class SecureAddressController_UT
    {

        [Fact]
        public async Task Get_Returns_Data()
        {
            // Arrange
            var expectedAddress = new Address { Id = 1, Zip = "90210" };            

            var repositoryMock = new MockAddressRepository();
            repositoryMock.GetAddressValue = expectedAddress;
            
            var webHost = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services => {
                        services.AddSingleton<IAddressRepository>(repositoryMock);
                        services.AddAuthentication("Test").AddJwtBearer("Test", options =>
                        {                            
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = false,
                                ValidateAudience = false,
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                RequireExpirationTime = true,
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this NOT our production key but a different one we use for tests"))                                
                            };
                            options.TokenValidationParameters.NameClaimType = "sub";
                        });
                    });
                });
            var client = webHost.CreateClient();

            var jwt = GenerateJwt();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");            

            // Act
            var response = await client.GetAsync($"api/secureaddress/{expectedAddress.Id}");          

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var addressResponse = await response.Content.ReadAsAsync<AddressRequest>();
            Assert.Equal(expectedAddress.Id, addressResponse.Id);
            Assert.Equal(expectedAddress.Zip, addressResponse.Zip);
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
            var addressRequest = new AddressRequest { Line1 = "L1 Post", City="Somewhere", State="KY", Zip = "22222" };
            var repositoryMock = new MockAddressRepository();

            var webHost = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services => {
                        services.AddSingleton<IAddressRepository>(repositoryMock);
                        services.AddAuthentication("Test").AddJwtBearer("Test", options =>
                        {
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = false,
                                ValidateAudience = false,
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                RequireExpirationTime = true,
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this NOT our production key but a different one we use for tests"))
                            };
                            options.TokenValidationParameters.NameClaimType = "sub";
                        });
                    });
                });
            var client = webHost.CreateClient();

            var jwt = GenerateJwt();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");

            // Act
            var response = await client.PostAsJsonAsync("api/secureaddress", addressRequest);

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
                        services.AddAuthentication("Test").AddJwtBearer("Test", options =>
                        {
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = false,
                                ValidateAudience = false,
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                RequireExpirationTime = true,
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this NOT our production key but a different one we use for tests"))
                            };
                            options.TokenValidationParameters.NameClaimType = "sub";
                        });
                    });
                });
            var client = webHost.CreateClient();

            var jwt = GenerateJwt();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");

            // Act
            var response = await client.PutAsJsonAsync($"api/secureaddress/{addressRequest.Id}", addressRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var addressUpdated = repositoryMock.UpdateAddressValue;  // This is the Address that was actually sent to the "Database"
            Assert.Equal(addressRequest.Line2, addressUpdated.Line2);
            Assert.Equal(addressRequest.Zip, addressUpdated.Zip);

            var addressResponse = await response.Content.ReadAsAsync<AddressRequest>();
            Assert.Equal(addressRequest.Line2, addressResponse.Line2);
            Assert.Equal(addressRequest.Zip, addressResponse.Zip);
        }

        private string GenerateJwt()
        {
            var secret = Encoding.UTF8.GetBytes("this NOT our production key but a different one we use for tests");
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
