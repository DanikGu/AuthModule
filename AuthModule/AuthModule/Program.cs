using AuthModule.AuthMiddleware;
using AuthModule.Extensions;
using AuthModule.Model;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(AuthScheme.DefaultScheme)
        .AddScheme<AuthOption, AuthHandler>(AuthScheme.DefaultScheme, null);
builder.Services.AddAuthorization();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(
    options =>
    {
        options.IdleTimeout = TimeSpan.FromDays(20);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    }
);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<UsersDbContext>(options =>
                         options.UseSqlite(builder.Configuration.GetConnectionString("UsersDb")));

var app = builder.Build();
app.UseSession();
app.UseAuth();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapGet("/", (HttpContext context) =>
{
    var user = context.User.Identity;
    if (user is not null && user.IsAuthenticated)
        return $"UserName: {user.Name}";
    else return "Пользователь не аутентифицирован.";
});

app.Run();
