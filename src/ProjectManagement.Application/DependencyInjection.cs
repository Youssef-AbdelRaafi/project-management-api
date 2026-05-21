using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Application.Common.Behaviors;

namespace ProjectManagement.Application;

/// <summary>
/// Registers application-layer services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds MediatR, FluentValidation, AutoMapper, and application pipeline behaviors.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(assembly);
        });

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
        services.AddAutoMapper(_ => { }, assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        return services;
    }
}
