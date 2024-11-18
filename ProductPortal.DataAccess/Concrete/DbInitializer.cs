// DataAccess/Concrete/DbInitializer.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductPortal.Core.Entities.Concrete;
using ProductPortal.Core.Utilities.Security;
using ProductPortal.DataAccess.Contexts;

namespace ProductPortal.DataAccess.Concrete
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ProductPortalContext>();
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("DbInitializer");

                try
                {
                    logger.LogInformation("Starting database initialization...");

                    // Veritabanını oluştur
                    await context.Database.MigrateAsync();

                    // Admin kullanıcısı var mı kontrol et
                    if (!await context.Users.AnyAsync(u => u.Role == "Admin"))
                    {
                        logger.LogInformation("Creating admin user...");

                        // Admin şifresi: "admin123"
                        byte[] passwordHash, passwordSalt;
                        HashingHelper.CreatePasswordHash("admin123", out passwordHash, out passwordSalt);

                        var adminUser = new User
                        {
                            Username = "admin",
                            Email = "admin@productportal.com",
                            PasswordHash = passwordHash,
                            PasswordSalt = passwordSalt,
                            Role = "Admin",
                            IsActive = true,
                            CreatedDate = DateTime.UtcNow
                        };

                        await context.Users.AddAsync(adminUser);
                        await context.SaveChangesAsync();

                        logger.LogInformation("Admin user created successfully");
                    }
                    else
                    {
                        logger.LogInformation("Admin user already exists");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while initializing the database");
                    throw;
                }
            }
        }
    }
}