using GE_Flipper;
using GE_Flipper.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHttpClient(); //Adds httpClient to build
builder.Services.AddHostedService<PriceGetter>(); //Adds background task to the build

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Configuring authentication services using credentials from appsettings.json
builder.Services.AddAuthentication()
    .AddGoogle(options =>
        {
            IConfigurationSection googleAuthSettings = builder.Configuration.GetSection("Authentication:Google");
            options.ClientId = googleAuthSettings["ClientId"];
            options.ClientSecret = googleAuthSettings["ClientSecret"];
        })
    .AddFacebook(options =>
            {
                IConfigurationSection facebookAuthSettings = builder.Configuration.GetSection("Authentication:Facebook");
                options.AppId = facebookAuthSettings["AppId"];
                options.AppSecret = facebookAuthSettings["AppSecret"];
            })
    .AddGitHub(options =>
        {
            IConfigurationSection gitHubAuthSettings = builder.Configuration.GetSection("Authentication:GitHub");
            options.ClientId = gitHubAuthSettings["ClientId"];
            options.ClientSecret = gitHubAuthSettings["ClientSecret"];
        });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.Use(async (context, next) => // Function for adding role to user after authentication
{
    if (context.User.Identity.IsAuthenticated) // If user is authenticated
    {
        var userManager = context.RequestServices.GetRequiredService<UserManager<IdentityUser>>(); // Variable for storin Identity user service

        var user = await userManager.GetUserAsync(context.User); // Finds user in DB and stores in variable
        if (user != null) // If user is not null 
        {
            var roles = await userManager.GetRolesAsync(user); // Finds user role and store in variable
            if (!roles.Contains("Customer") && !roles.Contains("Administrator")) // If role doesn't contain either customer or admin
            {
                await userManager.AddToRoleAsync(user, "Customer"); // Assign role to customer
            }
        }
    }
    await next();
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
