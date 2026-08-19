using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Kipas.Personel.API.Entities;
using Kipas.Personel.API.Helpers;

namespace Kipas.Personel.API.Data
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            var passwordHasher = scope.ServiceProvider
                .GetRequiredService<IPasswordHasher<AppUser>>();

            var username = configuration["AdminUser:Username"]?
                .Trim()
                .ToLowerInvariant();

            var password = configuration["AdminUser:Password"];

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Geliştirme ortamı yönetici bilgileri bulunamadı.");
            }

            var existingUser = await context.Users
                .FirstOrDefaultAsync(user =>
                    user.Username == username);

            if (existingUser != null)
            {
                if (existingUser.Role != RoleNames.Admin)
                {
                    throw new InvalidOperationException(
                        "Belirlenen yönetici kullanıcı adı normal bir hesaba ait. " +
                        "Güvenlik nedeniyle otomatik olarak Admin rolüne yükseltilmedi.");
                }

                // Admin zaten varsa yeniden oluşturulmaz
                // ve şifresi otomatik olarak değiştirilmez.
                return;
            }

            var adminUser = new AppUser
            {
                Username = username,
                Role = RoleNames.Admin,
                CreatedAt = DateTime.UtcNow
            };

            adminUser.PasswordHash =
                passwordHasher.HashPassword(
                    adminUser,
                    password);

            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();
        }
    }
}