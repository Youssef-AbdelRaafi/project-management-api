using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectManagement.Domain.Constants;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Infrastructure.Identity;

namespace ProjectManagement.Infrastructure.Persistence;

/// <summary>
/// Seeds required identity data.
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Seeds application roles and optionally a default administrator user.
    /// </summary>
    /// <param name="serviceProvider">The root service provider.</param>
    /// <returns>A task representing the asynchronous seed operation.</returns>
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(DbInitializer));

        foreach (var role in Roles.All)
        {
            if (await roleManager.RoleExistsAsync(role))
            {
                continue;
            }

            var result = await roleManager.CreateAsync(new IdentityRole(role));

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Failed to seed role '{role}': {errors}");
            }

            logger.LogInformation("Seeded identity role {Role}", role);
        }

        await SeedDefaultAdminAsync(userManager, configuration, logger);
    }

    private static async Task SeedDefaultAdminAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger logger)
    {
        var settings = configuration
            .GetSection(DefaultAdminSettings.SectionName)
            .Get<DefaultAdminSettings>();

        if (settings?.IsConfigured != true)
        {
            logger.LogInformation("Default admin seeding skipped because configuration is incomplete");
            return;
        }

        var existingUser = await userManager.FindByEmailAsync(settings.Email!);
        var adminUser = existingUser ?? ApplicationUser.Create(settings.Email!, settings.FullName!);

        if (existingUser is null)
        {
            var createResult = await userManager.CreateAsync(adminUser, settings.Password!);

            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Failed to seed default admin user: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, Roles.Admin))
        {
            var addRoleResult = await userManager.AddToRoleAsync(adminUser, Roles.Admin);

            if (!addRoleResult.Succeeded)
            {
                var errors = string.Join("; ", addRoleResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Failed to assign admin role to default admin user: {errors}");
            }
        }

        logger.LogInformation("Default admin user ensured for {Email}", settings.Email);
    }
}
