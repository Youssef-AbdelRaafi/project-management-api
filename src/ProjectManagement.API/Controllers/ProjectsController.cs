using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Projects.Commands.CreateProject;
using ProjectManagement.Application.Features.Projects.Commands.DeleteProject;
using ProjectManagement.Application.Features.Projects.Commands.UpdateProject;
using ProjectManagement.Application.Features.Projects.DTOs;
using ProjectManagement.Application.Features.Projects.Queries.GetAllProjects;
using ProjectManagement.Application.Features.Projects.Queries.GetProjectById;
using AspNetStatusCodes = Microsoft.AspNetCore.Http.StatusCodes;
using ResultStatusCodes = ProjectManagement.Application.Common.Models.StatusCodes;

namespace ProjectManagement.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class ProjectsController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Result<ProjectDto>), AspNetStatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProjectCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return StatusCode(
            result.StatusCode,
            result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<PaginatedList<ProjectDto>>), AspNetStatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllProjectsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(query, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Result<ProjectDetailsDto>), AspNetStatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProjectByIdQuery(id), cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Result<ProjectDto>), AspNetStatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProjectCommand(id, request.Name, request.Description);
        var result = await sender.Send(command, cancellationToken);

        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(AspNetStatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteProjectCommand(id), cancellationToken);

        return result.StatusCode == ResultStatusCodes.NoContent
            ? NoContent()
            : StatusCode(result.StatusCode, result);
    }

    public sealed record UpdateProjectRequest(string Name, string? Description);
}
