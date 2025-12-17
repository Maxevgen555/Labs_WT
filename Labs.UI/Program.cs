using Labs.UI.Data;
using Labs.UI.Services;
using Labs.UI.Services.Contracts;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<AppUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// Регистрация API сервисов
builder.Services.AddHttpClient<ICategoryService, ApiCategoryService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7002/api/categories/");
});

builder.Services.AddHttpClient<IProductService, ApiProductService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7002/api/dishes/");
});

// Для тэг-хелперов
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("admin", p =>
        p.RequireClaim(ClaimTypes.Role, "admin"));
});

builder.Services.AddSingleton<IEmailSender, NoOpEmailSender>();
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
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "catalog",
    pattern: "Catalog/{category?}/{pageNo?}",
    defaults: new { controller = "Product", action = "Index" });

app.MapControllerRoute(
    name: "image",
    pattern: "image/{action=GetAvatar}",
    defaults: new { controller = "Image" });

app.MapRazorPages();

await DbInit.SetupIdentityAdmin(app);

app.UseStaticFiles();

// обработка /Catalog как SPA маршрута:
app.MapWhen(context => context.Request.Path.StartsWithSegments("/Catalog"),
    appBuilder =>
    {
        appBuilder.Use((context, next) =>
        {
            context.Request.Path = "/";
            return next();
        });
        appBuilder.UseStaticFiles();
        appBuilder.UseRouting();
        appBuilder.UseEndpoints(endpoints =>
        {
            endpoints.MapFallbackToFile("/Catalog/{*path}", "index.html");
        });
    });

app.Run();