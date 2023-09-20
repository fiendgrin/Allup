using Allup.DataAccessLayer;
using Allup.Interfaces;
using Allup.Models;
using Allup.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<ILayoutService, LayoutService>();
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;

    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;

    options.Lockout.AllowedForNewUsers = false;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
})
    .AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();



var app = builder.Build();
app.MapControllerRoute("Area", "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
app.MapControllerRoute("Default","{controller=Home}/{action=Index}/{id?}");
app.UseSession();
app.UseStaticFiles();

app.Run();
