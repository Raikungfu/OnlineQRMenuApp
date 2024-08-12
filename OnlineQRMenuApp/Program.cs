using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using OnlineQRMenuApp.Models;
using OnlineQRMenuApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddDbContext<OnlineCoffeeManagementContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<AppHub<string>>("/AppHub/string");
app.MapHub<AppHub<Notification>>("/AppHub/noti");

app.Run();
