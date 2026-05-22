namespace ProjectManagement.Application.Features.Projects.DTOs;

/// <summary>
/// Represents a project summary.
/// </summary>
/// <param name="Id">The project identifier.</param>
/// <param name="Name">The project name.</param>
/// <param name="Description">The project description.</param>
/// <param name="CreatedAt">The UTC timestamp when the project was created.</param>
public sealed record ProjectDto(
    Guid Id,
    string Name,
    string? Description,
    DateTimeOffset CreatedAt);
