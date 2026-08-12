using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Common.Security;
using HotelManagement.Application.Features.Users.Contracts;
using HotelManagement.Application.Features.Users.Services;
using HotelManagement.Domain.Modules.Identity.Constants;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/users")]
public sealed class UsersController(
    UserAdminService service,
    ICurrentUser currentUser)
    : ControllerBase
{
    private Guid HotelId =>
        currentUser.RequireHotelId();

    // =========================================================
    // GET ALL
    // GET /api/v1/users
    // =========================================================

    [HasPermission(Permissions.Administration.UsersManage)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserResponse>>>> GetAll(
        CancellationToken cancellationToken)
    {
        var response =
            await service.GetAllAsync(
                HotelId,
                cancellationToken);

        return Ok(
            ApiResponse<IEnumerable<UserResponse>>.Ok(
                response,
                "Users retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // GET BY ID
    // GET /api/v1/users/{id}
    // =========================================================

    [HasPermission(Permissions.Administration.UsersManage)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> Get(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response =
            await service.GetByIdAsync(
                HotelId,
                id,
                cancellationToken);

        return Ok(
            ApiResponse<UserResponse>.Ok(
                response,
                "User retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE
    // POST /api/v1/users
    // =========================================================

    [HasPermission(Permissions.Administration.UsersManage)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserResponse>>> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var branchId =
            currentUser.RequireBranchId();

        var response =
            await service.CreateAsync(
                HotelId,
                branchId,
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(Get),
            new
            {
                id = response.Id
            },
            ApiResponse<UserResponse>.Created(
                response,
                "User created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // UPDATE
    // PUT /api/v1/users/{id}
    // =========================================================

    [HasPermission(Permissions.Administration.UsersManage)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> Update(
        Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var response =
            await service.UpdateAsync(
                HotelId,
                id,
                request,
                cancellationToken);

        return Ok(
            ApiResponse<UserResponse>.Updated(
                response,
                "User updated successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // ENABLE
    // POST /api/v1/users/{id}/enable
    // =========================================================

    [HasPermission(Permissions.Administration.UsersManage)]
    [HttpPost("{id:guid}/enable")]
    public async Task<ActionResult<ApiResponse<object?>>> Enable(
        Guid id,
        CancellationToken cancellationToken)
    {
        await service.SetActiveAsync(
            HotelId,
            id,
            true,
            cancellationToken);

        return Ok(
            ApiResponse<object?>.Action(
                null,
                "User enabled successfully.",
                "USER_ENABLED",
                HttpContext.TraceIdentifier));
    }

    // =========================================================
    // DISABLE
    // POST /api/v1/users/{id}/disable
    // =========================================================

    [HasPermission(Permissions.Administration.UsersManage)]
    [HttpPost("{id:guid}/disable")]
    public async Task<ActionResult<ApiResponse<object?>>> Disable(
        Guid id,
        CancellationToken cancellationToken)
    {
        await service.SetActiveAsync(
            HotelId,
            id,
            false,
            cancellationToken);

        return Ok(
            ApiResponse<object?>.Action(
                null,
                "User disabled successfully.",
                "USER_DISABLED",
                HttpContext.TraceIdentifier));
    }

    // =========================================================
    // RESET PASSWORD
    // POST /api/v1/users/{id}/reset-password
    // =========================================================

    [HasPermission(Permissions.Administration.UsersManage)]
    [HttpPost("{id:guid}/reset-password")]
    public async Task<ActionResult<ApiResponse<object?>>> ResetPassword(
        Guid id,
        [FromBody] ResetUserPasswordRequest request,
        CancellationToken cancellationToken)
    {
        await service.ResetPasswordAsync(
            HotelId,
            id,
            request.Password,
            cancellationToken);

        return Ok(
            ApiResponse<object?>.Action(
                null,
                "User password reset successfully.",
                "PASSWORD_RESET",
                HttpContext.TraceIdentifier));
    }

    // =========================================================
    // DELETE
    // DELETE /api/v1/users/{id}
    // =========================================================

    [HasPermission(Permissions.Administration.UsersManage)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await service.DeleteAsync(
            HotelId,
            id,
            cancellationToken);

        return NoContent();
    }
}