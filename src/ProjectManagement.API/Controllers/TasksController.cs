using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Tasks.Commands.CreateTask;
using ProjectManagement.Application.Features.Tasks.Commands.DeleteTask;
using ProjectManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;
using ProjectManagement.Application.Features.Tasks.DTOs;
using ProjectManagement.Application.Features.Tasks.Queries.GetTasksByProject;
using ProjectManagement.Domain.Enums;
using AspNetStatusCodes = Microsoft.AspNetCore.Http.StatusCodes;
using DomainTaskStatus = ProjectManagement.Domain.Enums.TaskStatus;
using ResultStatusCodes = ProjectManagement.Application.Common.Models.StatusCodes;

namespace ProjectManagement.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}")]
public sealed class TasksController(ISender sender) : ControllerBase
{
    [HttpPost("projects/{projectId:guid}/tasks")]
    [ProducesResponseType(typeof(Result<TaskDto>), AspNetStatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        Guid projectId,
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateTaskCommand(
            projectId,
            request.Title,
            request.Description,
            request.DueDate,
            request.Priority);

        var result = await sender.Send(command, cancellationToken);

        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("projects/{projectId:guid}/tasks")]
    [ProducesResponseType(typeof(Result<PaginatedList<TaskDto>>), AspNetStatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByProject(
        Guid projectId,
        [FromQuery] DomainTaskStatus? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetTasksByProjectQuery(projectId, status, pageNumber, pageSize), cancellationToken);

        return StatusCode(result.StatusCode, result);
    }

    [HttpPatch("tasks/{id:guid}/status")]
    [ProducesResponseType(typeof(Result<TaskDto>), AspNetStatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateTaskStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateTaskStatusCommand(id, request.Status),
            cancellationToken);

        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("tasks/{id:guid}")]
    [ProducesResponseType(AspNetStatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteTaskCommand(id), cancellationToken);

        return result.StatusCode == ResultStatusCodes.NoContent
            ? NoContent()
            : StatusCode(result.StatusCode, result);
    }

    public sealed record CreateTaskRequest(
        string Title,
        string? Description,
        DateTimeOffset DueDate,
        TaskPriority Priority);

    public sealed record UpdateTaskStatusRequest(DomainTaskStatus Status);
}
