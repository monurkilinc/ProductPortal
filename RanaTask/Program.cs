using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductPortal.Business.Abstract;
using ProductPortal.Business.Concrete;
using ProductPortal.Core.Middleware;
using ProductPortal.Core.Utilities.Security;
using ProductPortal.DataAccess.Abstract;
using ProductPortal.DataAccess.Concrete;
using ProductPortal.DataAccess.Contexts;
using System.Text;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Form dosya boyutu limitini ayarla
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue; // veya istediðiniz boyut
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue; // veya istediðiniz boyut
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

// Kep Handling ayarlarýný yapýlandýrma
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue; // veya istediðiniz boyut
});


// Temel servisler önce eklenmeli
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// API versiyonlama servisleri
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});


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
        ValidIssuer = tokenOptions.Issuer,
        ValidAudience = tokenOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)),
        ClockSkew = TimeSpan.Zero
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
//Swagger icin eklendi
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Product Portal API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


// Repository registrations
builder.Services.AddScoped<IProductRepository, EfProductRepository>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();

// Service registrations
builder.Services.AddScoped<ITokenHelper, JwtHelper>();
builder.Services.AddScoped<IAuthService, AuthManager>();
builder.Services.AddScoped<IProductService, ProductManager>();
builder.Services.AddScoped<IUserService, UserManager>();






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

//Swagger Middleware UI Kullanýmý icin eklendi
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Portal API v1"));
}

app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseMiddleware<AuthenticationMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Route and endpoints configuration
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Index}/{action=Admin}/{id?}");

app.Logger.LogInformation($"Application started at {DateTime.UtcNow}");
app.Run();
