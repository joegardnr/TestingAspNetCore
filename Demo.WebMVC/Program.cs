using Demo.WebMVC;

// The old "Startup.ConfigureServices" section.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IAddressRepository, AddressRepositoryInMemory>();
builder.Services.AddControllersWithViews();

// The old "Startup.Configure" section.
var app = builder.Build();
app.UseExceptionHandler("/Home/Error");
app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program { }  // Required for testing