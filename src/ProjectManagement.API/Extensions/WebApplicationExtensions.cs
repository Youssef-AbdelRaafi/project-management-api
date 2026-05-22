using Asp.Versioning.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Infrastructure.Persistence;
using Serilog;

namespace ProjectManagement.API.Extensions;

public static class WebApplicationExtensions
{
    private const int MigrationRetryCount = 5;
    private static readonly TimeSpan MigrationRetryDelay = TimeSpan.FromSeconds(5);

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
        app.MapHealthChecks("/health");
        app.MapControllers();

        return app;
    }

    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var cancellationToken = app.Lifetime.ApplicationStopping;

        for (var attempt = 1; attempt <= MigrationRetryCount; attempt++)
        {
            try
            {
                logger.LogInformation(
                    "Applying database migrations. Attempt {Attempt}/{RetryCount}",
                    attempt,
                    MigrationRetryCount);

                await dbContext.Database.MigrateAsync(cancellationToken);
                logger.LogInformation("Database migrations applied successfully");

                return;
            }
            catch (Exception exception) when (attempt < MigrationRetryCount)
            {
                logger.LogWarning(
                    exception,
                    "Database migration attempt {Attempt}/{RetryCount} failed. Retrying in {DelaySeconds} seconds",
                    attempt,
                    MigrationRetryCount,
                    MigrationRetryDelay.TotalSeconds);

                await Task.Delay(MigrationRetryDelay, cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Database migration failed");
                throw;
            }
        }
    }
}
