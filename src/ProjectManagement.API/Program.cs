using ProjectManagement.API.Extensions;
using ProjectManagement.Application;
using ProjectManagement.Infrastructure;
using ProjectManagement.Infrastructure.Persistence;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting ProjectManagement API");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext();
    });

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApiServices(builder.Configuration);

    var app = builder.Build();

    await app.ApplyMigrationsAsync();
    await DbInitializer.SeedRolesAsync(app.Services);
    app.UseApiPipeline();

    await app.RunAsync();
}
catch (Exception exception)
{
    if (exception.GetType().Name == "HostAbortedException")
    {
        return;
    }

    Log.Fatal(exception, "ProjectManagement API terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
