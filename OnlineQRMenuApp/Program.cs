using Microsoft.EntityFrameworkCore;
using OnlineQRMenuApp.Models;
using OnlineQRMenuApp.Hubs;
using OnlineQRMenuApp.Dto;
using OnlineQRMenuApp.Models.ViewModel;
using OnlineQRMenuApp.Service;
using System.Security.Claims;
using System.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").GetChildren().FirstOrDefault(x => x.Key == builder.Environment.EnvironmentName)?.Value?.Split(',');

builder.Services.AddDbContext<OnlineCoffeeManagementContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.IgnoreNullValues = true;
    });

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication("CoffeeShopSaintRai")
    .AddCookie("CoffeeShopSaintRai", options =>
    {
        options.LoginPath = "/Members/Login";
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "CoffeeShopSaintRai",
            ValidAudience = "CoffeeShopSaintRai",
            IssuerSigningKey = new RsaSecurityKey(KeyHelper.GetPrivateKey())
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Admin"));
    options.AddPolicy("CoffeeShopManagerPolicy", policy =>
        policy.RequireClaim(ClaimTypes.Role, "CoffeeShopManager"));
    options.AddPolicy("CoffeeShopCustomerPolicy", policy =>
            policy.RequireClaim(ClaimTypes.Role, "CoffeeShopCustomer"));
});


builder.Services.AddScoped<TokenService>();


builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
            .SetIsOriginAllowed(_ => true)
              .AllowCredentials();
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

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

app.MapHub<AppHub<string>>("/AppHub");
app.MapHub<AppHub<Notification>>("/AppHub/noti");
app.MapHub<AppHub<OrderProcessDto>>("/AppHub/order");

app.Run();