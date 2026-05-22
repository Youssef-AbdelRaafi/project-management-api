using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ProjectManagement.Infrastructure.Persistence;

public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseSqlServer(
            GetConnectionString(),
            sqlOptions => sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

        return new ApplicationDbContext(optionsBuilder.Options);
    }

    private static string GetConnectionString()
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(GetConfigurationBasePath())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        return configuration.GetConnectionString("DefaultConnection")
            ?? Environment.GetEnvironmentVariable("SQLCONNSTR_DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
    }

    private static string GetConfigurationBasePath()
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        var apiProjectPath = Path.Combine(currentDirectory, "src", "ProjectManagement.API");
        if (Directory.Exists(apiProjectPath))
        {
            return apiProjectPath;
        }

        apiProjectPath = Path.GetFullPath(Path.Combine(currentDirectory, "..", "ProjectManagement.API"));
        if (Directory.Exists(apiProjectPath))
        {
            return apiProjectPath;
        }

        return currentDirectory;
    }
}
