using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Auth.DTOs;
using ProjectManagement.Domain.Constants;
using AspNetStatusCodes = Microsoft.AspNetCore.Http.StatusCodes;

using MediatR;
using ProjectManagement.Application.Features.Users.Queries.GetAllUsers;

namespace ProjectManagement.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = Roles.Admin)]
public sealed class AdminController(ISender sender) : ControllerBase
{
    [HttpGet("users")]
    [ProducesResponseType(typeof(Result<IReadOnlyCollection<UserDto>>), AspNetStatusCodes.Status200OK)]
    [ProducesResponseType(AspNetStatusCodes.Status401Unauthorized)]
    [ProducesResponseType(AspNetStatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllUsersQuery(), cancellationToken);
        return Ok(result);
    }
}
