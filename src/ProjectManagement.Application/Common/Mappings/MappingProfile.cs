using AutoMapper;
using ProjectManagement.Application.Features.Projects.DTOs;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Common.Mappings;

/// <summary>
/// Central AutoMapper profile for application DTO mappings.
/// </summary>
public sealed class MappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappingProfile" /> class.
    /// </summary>
    public MappingProfile()
    {
        CreateMap<Project, ProjectDto>();
        CreateMap<Project, ProjectDetailsDto>()
            .ForCtorParam(
                nameof(ProjectDetailsDto.TasksCount),
                options => options.MapFrom(project => project.Tasks.Count));
    }
}
