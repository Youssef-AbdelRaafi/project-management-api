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

/// <summary>
/// Project management endpoints for authenticated users.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class ProjectsController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Creates a project for the authenticated user.
    /// </summary>
    /// <param name="command">The create project request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created project.</returns>
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

    /// <summary>
    /// Gets projects owned by the authenticated user.
    /// </summary>
    /// <param name="query">The project list query parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated project list.</returns>
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

    /// <summary>
    /// Gets one project owned by the authenticated user.
    /// </summary>
    /// <param name="id">The project identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The project details.</returns>
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

    /// <summary>
    /// Updates one project owned by the authenticated user.
    /// </summary>
    /// <param name="id">The project identifier.</param>
    /// <param name="request">The update project request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated project.</returns>
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

    /// <summary>
    /// Deletes one project owned by the authenticated user.
    /// </summary>
    /// <param name="id">The project identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>No content when the project is deleted.</returns>
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

    /// <summary>
    /// Request body used when updating a project.
    /// </summary>
    /// <param name="Name">The project name.</param>
    /// <param name="Description">The project description.</param>
    public sealed record UpdateProjectRequest(string Name, string? Description);
}
