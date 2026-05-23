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

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class AuthController(ISender sender) : ControllerBase
{
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

    [HttpPost("logout")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    [ProducesResponseType(AspNetStatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<object>), AspNetStatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(
        [FromBody] ProjectManagement.Application.Features.Auth.Commands.Logout.LogoutCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : StatusCode(result.StatusCode, result);
    }

    private ObjectResult ToActionResult(Result<AuthResponseDto> result)
    {
        return StatusCode(result.StatusCode, result);
    }
}
