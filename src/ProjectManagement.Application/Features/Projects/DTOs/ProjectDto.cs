namespace ProjectManagement.Application.Features.Projects.DTOs;

public sealed record ProjectDto(
    Guid Id,
    string Name,
    string? Description,
    DateTimeOffset CreatedAt);
