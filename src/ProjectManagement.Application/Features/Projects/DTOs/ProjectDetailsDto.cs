namespace ProjectManagement.Application.Features.Projects.DTOs;

/// <summary>
/// Represents detailed project information.
/// </summary>
/// <param name="Id">The project identifier.</param>
/// <param name="Name">The project name.</param>
/// <param name="Description">The project description.</param>
/// <param name="CreatedAt">The UTC timestamp when the project was created.</param>
/// <param name="UpdatedAt">The UTC timestamp when the project was last updated.</param>
/// <param name="TasksCount">The number of tasks inside the project.</param>
public sealed record ProjectDetailsDto(
    Guid Id,
    string Name,
    string? Description,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    int TasksCount);
