using AutoMapper;
using ProjectManagement.Application.Features.Projects.DTOs;
using ProjectManagement.Application.Features.Tasks.DTOs;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Common.Mappings;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Project, ProjectDto>();
        CreateMap<Project, ProjectDetailsDto>()
            .ForCtorParam(
                nameof(ProjectDetailsDto.TasksCount),
                options => options.MapFrom(project => project.Tasks.Count));

        CreateMap<TaskItem, TaskDto>();
    }
}
