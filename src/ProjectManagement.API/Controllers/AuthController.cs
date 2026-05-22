using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Auth.Commands.Login;
using ProjectManagement.Application.Features.Auth.Commands.RefreshToken;
using ProjectManagement.Application.Features.Auth.Commands.Register;
using ProjectManagement.Application.Features.Auth.DTOs;
using AspNetStatusCodes = Microsoft.AspNetCore.Http.StatusCodes;

namespace ProjectManagement.API.Controllers;

/// <summary>
/// Authentication endpoints for registration, login, and refresh-token rotation.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="command">The registration request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The issued authentication token pair.</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(Result<AuthResponseDto>), AspNetStatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<AuthResponseDto>), AspNetStatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return ToActionResult(result);
    }

    /// <summary>
    /// Authenticates an existing user.
    /// </summary>
    /// <param name="command">The login request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The issued authentication token pair.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(Result<AuthResponseDto>), AspNetStatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<AuthResponseDto>), AspNetStatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<AuthResponseDto>), AspNetStatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return ToActionResult(result);
    }

    /// <summary>
    /// Rotates a refresh token and issues a new token pair.
    /// </summary>
    /// <param name="command">The refresh-token rotation request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The issued authentication token pair.</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(Result<AuthResponseDto>), AspNetStatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<AuthResponseDto>), AspNetStatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<AuthResponseDto>), AspNetStatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return ToActionResult(result);
    }

    private ObjectResult ToActionResult(Result<AuthResponseDto> result)
    {
        return StatusCode(result.StatusCode, result);
    }
}
