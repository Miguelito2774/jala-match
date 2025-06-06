using Application.Abstractions.Data;
using Domain.Entities.Enums;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Seeders;

public static class AdminSeeder
{
    public static async Task SeedAdminUser(IApplicationDbContext context)
    {
        if (!await context.Users.AnyAsync(u => u.Role == Role.Admin))
        {
            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@teamgenerator.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = Role.Admin,
                ProfilePictureUrl = null,
            };

            context.Users.Add(adminUser);
            await context.SaveChangesAsync(CancellationToken.None);
        }
    }
}
