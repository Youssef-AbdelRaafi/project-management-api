using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Auth.DTOs;
using ProjectManagement.Domain.Constants;
using ProjectManagement.Domain.Entities;
using AspNetStatusCodes = Microsoft.AspNetCore.Http.StatusCodes;

namespace ProjectManagement.API.Controllers;

/// <summary>
/// Administrative endpoints demonstrating role-based authorization.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = Roles.Admin)]
public sealed class AdminController(UserManager<ApplicationUser> userManager) : ControllerBase
{
    /// <summary>
    /// Lists all application users. Requires the Admin role.
    /// </summary>
    /// <returns>The application users.</returns>
    [HttpGet("users")]
    [ProducesResponseType(typeof(Result<IReadOnlyCollection<UserDto>>), AspNetStatusCodes.Status200OK)]
    [ProducesResponseType(AspNetStatusCodes.Status401Unauthorized)]
    [ProducesResponseType(AspNetStatusCodes.Status403Forbidden)]
    public IActionResult GetUsers()
    {
        var users = userManager.Users
            .Select(user => new UserDto(user.Id, user.Email ?? string.Empty, user.FullName))
            .ToList();

        return Ok(Result<IReadOnlyCollection<UserDto>>.Success(users));
    }
}
