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

//  Google authentication to read ClientId and ClientSecret from appsettings.json to allow Google sign in
builder.Services.AddAuthentication()
    .AddGoogle(options =>
        {
            IConfigurationSection googleAuthSettings = builder.Configuration.GetSection("Authentication:Google");
            options.ClientId = googleAuthSettings["ClientId"];
            options.ClientSecret = googleAuthSettings["ClientSecret"];
        })
    // Facebook authentication to read AppId and AppSecret from appsettings.json to allow Facebook sign in
    .AddFacebook(options =>
            {
                IConfigurationSection facebookAuthSettings = builder.Configuration.GetSection("Authentication:Facebook");
                options.AppId = facebookAuthSettings["AppId"];
                options.AppSecret = facebookAuthSettings["AppSecret"];
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

app.Use(async (context, next) => //function for adding role to user after authentication
{
    if (context.User.Identity.IsAuthenticated) //If user is authenticated
    {
        var userManager = context.RequestServices.GetRequiredService<UserManager<IdentityUser>>(); //Variable for storin Identity user service

        var user = await userManager.GetUserAsync(context.User); //Finds user in DB and stores in variable
        if (user != null) //If user is not null 
        {
            var roles = await userManager.GetRolesAsync(user); //Finds user role and store in variable
            if (!roles.Contains("Customer") && !roles.Contains("Administrator")) //If role doesn't contain either customer or admin
            {
                await userManager.AddToRoleAsync(user, "Customer"); //Assign role to customer
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
