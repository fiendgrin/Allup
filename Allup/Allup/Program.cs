using Allup.DataAccessLayer;
using Allup.Interfaces;
using Allup.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<ILayoutService, LayoutService>();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();



var app = builder.Build();
app.MapControllerRoute("Area", "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
app.MapControllerRoute("Default","{controller=Home}/{action=Index}/{id?}");
app.UseSession();
app.UseStaticFiles();

app.Run();
