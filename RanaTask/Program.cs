using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductPortal.Business.Abstract;
using ProductPortal.Business.Concrete;
using ProductPortal.Core.Utilities.Results;
using ProductPortal.Core.Utilities.Security;
using ProductPortal.DataAccess.Abstract;
using ProductPortal.DataAccess.Concrete;
using ProductPortal.DataAccess.Contexts;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<ProductPortalContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.MigrationsAssembly("ProductPortal.Web")
    )
);

builder.Services.Configure<TokenOptions>(builder.Configuration.GetSection("TokenOptions"));

builder.Services.AddScoped<ITokenHelper, JwtHelper>();
builder.Services.AddScoped<IProductRepository, EfProductRepository>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<IAuthService, AuthManager>();

//Servislerin DI edilmesi   
builder.Services.AddScoped<IProductService, ProductManager>();

// JWT Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Result.cs de kullanýlan IHttpContextAccessor servisi icin gerekli
builder.Services.AddHttpContextAccessor();

//CORS politikasi
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

//Result.cs de kullandýðýmýz ILogger servisi icin gerekli
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    //Loglarda timestamp formatýný ayarlar
    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
});
builder.Logging.AddDebug();

//Log seviyelerinin yapilandirilmasi
builder.Logging.SetMinimumLevel(LogLevel.Information);

//Microsoft ve System loglarýnýn LogLevel.Warning seviyesinde loglanmasi
builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
builder.Logging.AddFilter("System", LogLevel.Warning);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//TraceId ve Timestamp duzgun calismasi icin middleware eklendi
//app.Use(async (context, next) =>
//{
//    //TraceId'yi HTTP context'e eklendi
//    context.TraceIdentifier = Guid.NewGuid().ToString();

//    await next();
//});
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAuthorization();
app.MapControllers();
app.UseCors("AllowAll");


//Uygulamanin baslangic logu
app.Logger.LogInformation($"Uygulama {DateTime.UtcNow} Basladi. ");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Index}/{id?}");
app.Run();
