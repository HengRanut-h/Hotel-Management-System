using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Application.Features.Profile.Services;
using HotelManagement.Application.Features.Users.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[Authorize]
[ApiController]
[Route("api/v1/profile")]
public sealed class ProfileController : ControllerBase
{
    private readonly ProfileService _profileService;
    private readonly ICurrentUser _currentUser;

    public ProfileController(
        ProfileService profileService,
        ICurrentUser currentUser)
    {
        _profileService = profileService;
        _currentUser = currentUser;
    }

    // =========================================================
    // GET CURRENT USER PROFILE
    // GET: /api/v1/profile
    // =========================================================

    [HttpGet]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> Get(
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        if (userId is null)
        {
            throw new ForbiddenException();
        }

        var profile = await _profileService.GetAsync(
            userId.Value,
            cancellationToken);

        return Ok(profile);
    }
}