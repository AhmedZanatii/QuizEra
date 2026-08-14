using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuizEra.BLL.Services.Auth;
using QuizEra.DAL.DataBase;
using QuizEra.DAL.Entities;
using QuizEra.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString =
    builder.Configuration.GetConnectionString("Zanatii");

builder.Services.AddDbContext<QuizEraDBContext>(options =>
    options.UseSqlServer(connectionString));

// Identity Configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // SignIn Configuration
    options.SignIn.RequireConfirmedAccount = false;

    // Password Configuration
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 0;
})
.AddEntityFrameworkStores<QuizEraDBContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = new PathString("/Account/Login");
        options.AccessDeniedPath = new PathString("/Account/AccessDenied");
    });

builder.Services.AddScoped<AuthService>();

var app = builder.Build();
//role (authentication) seeding
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    await RoleSeeder.SeedRolesAsync(roleManager);
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

// Authentication → Who are you?
app.UseAuthentication();

// Authorization → What are you allowed to do?
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();