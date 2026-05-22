namespace ProjectManagement.Application.Features.Projects.DTOs;

public sealed record ProjectDetailsDto(
    Guid Id,
    string Name,
    string? Description,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    int TasksCount);
