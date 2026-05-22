using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Auth.DTOs;
using ProjectManagement.Domain.Constants;
using AspNetStatusCodes = Microsoft.AspNetCore.Http.StatusCodes;

namespace ProjectManagement.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = Roles.Admin)]
public sealed class AdminController(IIdentityService identityService) : ControllerBase
{
    [HttpGet("users")]
    [ProducesResponseType(typeof(Result<IReadOnlyCollection<UserDto>>), AspNetStatusCodes.Status200OK)]
    [ProducesResponseType(AspNetStatusCodes.Status401Unauthorized)]
    [ProducesResponseType(AspNetStatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
    {
        var users = await identityService.GetAllUsersAsync(cancellationToken);

        var userDtos = users
            .Select(user => new UserDto(user.Id, user.Email, user.FullName))
            .ToList();

        return Ok(Result<IReadOnlyCollection<UserDto>>.Success(userDtos));
    }
}
