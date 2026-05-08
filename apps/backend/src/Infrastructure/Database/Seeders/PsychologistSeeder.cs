using Application.Abstractions.Data;
using Domain.Entities.Enums;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Seeders;

public static class PsychologistSeeder
{
    public static async Task SeedPsychologistUser(IApplicationDbContext context)
    {
        if (!await context.Users.AnyAsync(u => u.Role == Role.Psychologist))
        {
            var psychologistUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "psychologist@teamgenerator.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Psychologist123!"),
                Role = Role.Psychologist,
                ProfilePictureUrl = null,
            };

            context.Users.Add(psychologistUser);
            await context.SaveChangesAsync(CancellationToken.None);
        }
    }
}
