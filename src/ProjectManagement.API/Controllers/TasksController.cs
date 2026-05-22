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

/// <summary>
/// Task management endpoints for authenticated users.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}")]
public sealed class TasksController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Creates a task inside a project owned by the authenticated user.
    /// </summary>
    /// <param name="projectId">The parent project identifier.</param>
    /// <param name="request">The create task request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created task.</returns>
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

    /// <summary>
    /// Gets tasks inside a project owned by the authenticated user.
    /// </summary>
    /// <param name="projectId">The parent project identifier.</param>
    /// <param name="status">Optional workflow status filter.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The project tasks.</returns>
    [HttpGet("projects/{projectId:guid}/tasks")]
    [ProducesResponseType(typeof(Result<IReadOnlyCollection<TaskDto>>), AspNetStatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByProject(
        Guid projectId,
        [FromQuery] DomainTaskStatus? status,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetTasksByProjectQuery(projectId, status), cancellationToken);

        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Updates only the workflow status of one task owned through its project.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <param name="request">The status update request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated task.</returns>
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

    /// <summary>
    /// Deletes one task owned through its project.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>No content when the task is deleted.</returns>
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

    /// <summary>
    /// Request body used when creating a task.
    /// </summary>
    /// <param name="Title">The task title.</param>
    /// <param name="Description">The task description.</param>
    /// <param name="DueDate">The task due date.</param>
    /// <param name="Priority">The task priority.</param>
    public sealed record CreateTaskRequest(
        string Title,
        string? Description,
        DateTime DueDate,
        TaskPriority Priority);

    /// <summary>
    /// Request body used when updating task status.
    /// </summary>
    /// <param name="Status">The new workflow status.</param>
    public sealed record UpdateTaskStatusRequest(DomainTaskStatus Status);
}
