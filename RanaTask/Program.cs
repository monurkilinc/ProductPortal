using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductPortal.Business.Abstract;
using ProductPortal.Business.Concrete;
using ProductPortal.Core.Middleware;
using ProductPortal.Core.Utilities.Results;
using ProductPortal.Core.Utilities.Security;
using ProductPortal.DataAccess.Abstract;
using ProductPortal.DataAccess.Concrete;
using ProductPortal.DataAccess.Contexts;
using Serilog;
using System.Configuration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container
builder.Services.AddControllersWithViews();

// Database Configuration
builder.Services.AddDbContext<ProductPortalContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.MigrationsAssembly("ProductPortal.Web")
    )
);

// JWT Configuration
var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>();
builder.Services.Configure<TokenOptions>(builder.Configuration.GetSection("TokenOptions"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["TokenOptions:Issuer"],
        ValidAudience = builder.Configuration["TokenOptions:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["TokenOptions:SecurityKey"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["jwt"];
            return Task.CompletedTask;
        }
    };
});
// Dependency Injection
builder.Services.AddScoped<ITokenHelper, JwtHelper>();
builder.Services.AddScoped<IProductRepository, EfProductRepository>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<IAuthService, AuthManager>();
builder.Services.AddScoped<IProductService, ProductManager>();

// Additional Services
builder.Services.AddHttpContextAccessor();


// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Logging Configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
});
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
builder.Logging.AddFilter("System", LogLevel.Warning);

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();

await DbInitializer.Initialize(app.Services);

// Database Initialization
try
{
    await DbInitializer.Initialize(app.Services);
    app.Logger.LogInformation("Database initialized successfully");
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "An error occurred while initializing the database");
}


app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuthenticationMiddleware>();

// Route Configuration
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Index}/{action=Admin}/{id?}");

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}


app.Logger.LogInformation($"Application started at {DateTime.UtcNow}");

app.Run();