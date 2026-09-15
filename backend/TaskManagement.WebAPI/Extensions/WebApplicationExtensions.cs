using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TaskManagement.AppServices.Authentication;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.WebAPI.Extensions;

public static class WebApplicationExtensions
{
    public static async Task SeedDevelopmentDataAsync(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return;
        }

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagementDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IUserPasswordHasher>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<DevelopmentSeedOptions>>().Value;
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DevelopmentSeed");

        if (options.Users.Count == 0)
        {
            logger.LogInformation("Development user seed skipped because no users were configured.");
            return;
        }

        foreach (var seedUser in options.Users)
        {
            if (string.IsNullOrWhiteSpace(seedUser.Email)
                || string.IsNullOrWhiteSpace(seedUser.Password)
                || !Enum.TryParse<UserRole>(seedUser.Role, true, out var role))
            {
                logger.LogWarning("Skipping invalid development seed user entry for email '{Email}'.", seedUser.Email);
                continue;
            }

            var normalizedEmail = seedUser.Email.Trim().ToLowerInvariant();
            var existingUser = await dbContext.Users.FirstOrDefaultAsync(user => user.Email == normalizedEmail);
            var firstName = seedUser.FirstName.Trim();
            var lastName = seedUser.LastName.Trim();

            if (existingUser is not null)
            {
                existingUser.FirstName = firstName;
                existingUser.LastName = lastName;
                existingUser.Role = role;
                existingUser.IsActive = seedUser.IsActive;
                existingUser.PasswordHash = passwordHasher.HashPassword(existingUser, seedUser.Password);
                existingUser.UpdatedAtUtc = DateTime.UtcNow;
                continue;
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = normalizedEmail,
                FirstName = firstName,
                LastName = lastName,
                Role = role,
                IsActive = seedUser.IsActive
            };

            user.PasswordHash = passwordHasher.HashPassword(user, seedUser.Password);
            await dbContext.Users.AddAsync(user);
        }

        await dbContext.SaveChangesAsync();
        logger.LogInformation("Development user seed completed.");
    }
}