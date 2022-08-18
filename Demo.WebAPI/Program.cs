using Demo.WebAPI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// The old "Startup.ConfigureServices" section.
builder.Services.AddControllers();
builder.Services.AddSingleton<IAddressRepository, AddressRepositoryInMemory>();
builder.Services.AddSingleton<IValidator, Validator>();
builder.Services.AddSingleton<IMapper, Mapper>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        RequireExpirationTime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("a really super secret key that needs to be exactly 64 bytes long"))
    };
    options.TokenValidationParameters.NameClaimType = "sub";
});

// The old "Startup.Configure" section.
var app = builder.Build();
app.Use(async (context, next) =>
{
    Console.WriteLine("Middleware!");
    await next.Invoke();
});
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }  // Required for testing