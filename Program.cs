using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LingoToneMVC.Data;
using LingoToneMVC.Models;

var builder = WebApplication.CreateBuilder(args);

// ========== SERVICES ==========
// Database
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
});

// MVC
builder.Services.AddControllersWithViews();

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Seeders
builder.Services.AddTransient<LingoToneMVC.Services.DataSeeder>();
builder.Services.AddScoped<LingoToneMVC.Services.HskLessonService>();

// AI Services
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<LingoToneMVC.Services.IAiService, LingoToneMVC.Services.GeminiAiService>();

var app = builder.Build();

// ========== PIPELINE ==========
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// ========== ROUTES ==========
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ========== DATABASE MIGRATION ==========
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.IsRelational())
    {
        db.Database.Migrate();
    }
    
    // Run Data Seeder
    var seeder = scope.ServiceProvider.GetRequiredService<LingoToneMVC.Services.DataSeeder>();
    seeder.SeedAsync().Wait();
}

app.Run();

public partial class Program { }
