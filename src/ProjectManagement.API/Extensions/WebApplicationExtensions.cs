using Asp.Versioning.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Infrastructure.Persistence;
using Serilog;

namespace ProjectManagement.API.Extensions;

/// <summary>
/// Configures the API request pipeline and startup database tasks.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Configures middleware for exception handling, Swagger, CORS, authentication, and controllers.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The same web application for chaining.</returns>
    public static WebApplication UseApiPipeline(this WebApplication app)
    {
        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            var apiVersionDescriptionProvider = app.Services
                .GetRequiredService<IApiVersionDescriptionProvider>();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }

                options.DisplayRequestDuration();
            });
        }

        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
                "Handled HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        });

        app.UseHttpsRedirection();
        app.UseCors(ServiceCollectionExtensions.CorsPolicyName);
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }

    /// <summary>
    /// Applies pending EF Core migrations at application startup.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>A task representing the asynchronous migration operation.</returns>
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            logger.LogInformation("Applying database migrations");
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Database migration failed");
            throw;
        }
    }
}
