using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Infrastructure.Identity;
using ProjectManagement.Infrastructure.Persistence;
using ProjectManagement.Infrastructure.Persistence.Interceptors;

namespace ProjectManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddHttpContextAccessor();

        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<SoftDeleteInterceptor>();

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var auditableEntityInterceptor = serviceProvider.GetRequiredService<AuditableEntityInterceptor>();
            var softDeleteInterceptor = serviceProvider.GetRequiredService<SoftDeleteInterceptor>();

            options.UseSqlServer(
                connectionString,
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

            options.AddInterceptors(softDeleteInterceptor, auditableEntityInterceptor);
        });

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services
            .AddOptions<JwtSettings>()
            .Bind(configuration.GetSection(JwtSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}
