using System.Security.Claims;

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
    // =========================================================
    // CURRENT ROLE NAMES
    // =========================================================

    private IReadOnlyCollection<string>
        CurrentRoleNames
    {
        get
        {
            var roles =
                new HashSet<string>(
                    StringComparer
                        .OrdinalIgnoreCase);

            foreach (
                var claim in
                User.Claims)
            {
                var isRoleClaim =
                    claim.Type ==
                    ClaimTypes.Role
                    ||
                    claim.Type.Equals(
                        "role",
                        StringComparison
                            .OrdinalIgnoreCase)
                    ||
                    claim.Type.Equals(
                        "roles",
                        StringComparison
                            .OrdinalIgnoreCase);

                if (!isRoleClaim)
                {
                    continue;
                }

                var values =
                    claim.Value.Split(
                        ',',
                        StringSplitOptions
                            .RemoveEmptyEntries
                        |
                        StringSplitOptions
                            .TrimEntries);

                foreach (
                    var value in values)
                {
                    roles.Add(
                        value);
                }
            }

            return roles;
        }
    }

    // =========================================================
    // CURRENT USER ID
    //
    // Used to prevent a user managing their own account
    // through administration endpoints.
    // =========================================================

    private Guid? CurrentUserId
    {
        get
        {
            var value =
                User.Claims
                    .FirstOrDefault(
                        claim =>
                            claim.Type ==
                            ClaimTypes
                                .NameIdentifier
                            ||
                            claim.Type.Equals(
                                "sub",
                                StringComparison
                                    .OrdinalIgnoreCase)
                            ||
                            claim.Type.Equals(
                                "user_id",
                                StringComparison
                                    .OrdinalIgnoreCase))
                    ?.Value;

            return Guid.TryParse(
                value,
                out var userId)
                ? userId
                : null;
        }
    }

    // =========================================================
    // SUPER ADMIN
    // =========================================================

    private bool IsSuperAdmin =>
        RoleAccessPolicy
            .IsSuperAdmin(
                CurrentRoleNames);

    // =========================================================
    // HOTEL SCOPE
    //
    // SuperAdmin = global
    // Others = current hotel
    // =========================================================

    private Guid? ScopeHotelId =>
        IsSuperAdmin
            ? null
            : currentUser
                .RequireHotelId();

    // =========================================================
    // GET ALL
    // =========================================================

    [HasPermission(
        Permissions.Administration.UsersManage)]
    [HttpGet]
    public async Task<
        ActionResult<
            ApiResponse<
                IEnumerable<UserResponse>>>>
        GetAll(
            CancellationToken cancellationToken)
    {
        var response =
            await service.GetAllAsync(
                ScopeHotelId,
                CurrentRoleNames,
                CurrentUserId,
                cancellationToken);

        return Ok(
            ApiResponse<
                IEnumerable<UserResponse>>
                .Ok(
                    response,
                    "Users retrieved successfully.",
                    traceId:
                        HttpContext
                            .TraceIdentifier));
    }

    // =========================================================
    // ASSIGNABLE ROLES
    //
    // GET /api/v1/users/assignable-roles
    //
    // HotelAdmin:
    // SuperAdmin      ❌
    // HotelAdmin peer ❌
    // Manager         ✅
    // Staff           ✅
    //
    // HRManager:
    // HRStaff         ✅
    // Accountant      ❌
    // =========================================================

    [HasPermission(
        Permissions.Administration.UsersManage)]
    [HttpGet("assignable-roles")]
    public async Task<
        ActionResult<
            ApiResponse<
                IEnumerable<string>>>>
        GetAssignableRoles(
            CancellationToken cancellationToken)
    {
        var roles =
            await service
                .GetAssignableRolesAsync(
                    CurrentRoleNames,
                    cancellationToken);

        return Ok(
            ApiResponse<
                IEnumerable<string>>
                .Ok(
                    roles,
                    "Assignable roles retrieved successfully.",
                    traceId:
                        HttpContext
                            .TraceIdentifier));
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    [HasPermission(
        Permissions.Administration.UsersManage)]
    [HttpGet("{id:guid}")]
    public async Task<
        ActionResult<
            ApiResponse<UserResponse>>>
        Get(
            Guid id,
            CancellationToken cancellationToken)
    {
        var response =
            await service.GetByIdAsync(
                ScopeHotelId,
                CurrentRoleNames,
                CurrentUserId,
                id,
                cancellationToken);

        return Ok(
            ApiResponse<UserResponse>.Ok(
                response,
                "User retrieved successfully.",
                traceId:
                    HttpContext
                        .TraceIdentifier));
    }

    // =========================================================
    // CREATE
    //
    // Current create flow creates a user inside the
    // authenticated admin's hotel/branch.
    // =========================================================

    [HasPermission(
        Permissions.Administration.UsersManage)]
    [HttpPost]
    public async Task<
        ActionResult<
            ApiResponse<UserResponse>>>
        Create(
            [FromBody]
            CreateUserRequest request,
            CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser
                .RequireHotelId();

        var branchId =
            currentUser
                .RequireBranchId();

        var response =
            await service.CreateAsync(
                hotelId,
                branchId,
                CurrentRoleNames,
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(Get),
            new
            {
                id =
                    response.Id
            },
            ApiResponse<UserResponse>.Created(
                response,
                "User created successfully.",
                traceId:
                    HttpContext
                        .TraceIdentifier));
    }

    // =========================================================
    // UPDATE
    // =========================================================

    [HasPermission(
        Permissions.Administration.UsersManage)]
    [HttpPut("{id:guid}")]
    public async Task<
        ActionResult<
            ApiResponse<UserResponse>>>
        Update(
            Guid id,
            [FromBody]
            UpdateUserRequest request,
            CancellationToken cancellationToken)
    {
        var response =
            await service.UpdateAsync(
                ScopeHotelId,
                CurrentRoleNames,
                CurrentUserId,
                id,
                request,
                cancellationToken);

        return Ok(
            ApiResponse<UserResponse>.Updated(
                response,
                "User updated successfully.",
                traceId:
                    HttpContext
                        .TraceIdentifier));
    }

    // =========================================================
    // ENABLE
    // =========================================================

    [HasPermission(
        Permissions.Administration.UsersManage)]
    [HttpPost("{id:guid}/enable")]
    public async Task<
        ActionResult<
            ApiResponse<object?>>>
        Enable(
            Guid id,
            CancellationToken cancellationToken)
    {
        await service.SetActiveAsync(
            ScopeHotelId,
            CurrentRoleNames,
            CurrentUserId,
            id,
            true,
            cancellationToken);

        return Ok(
            ApiResponse<object?>.Action(
                null,
                "User enabled successfully.",
                "USER_ENABLED",
                HttpContext
                    .TraceIdentifier));
    }

    // =========================================================
    // DISABLE
    // =========================================================

    [HasPermission(
        Permissions.Administration.UsersManage)]
    [HttpPost("{id:guid}/disable")]
    public async Task<
        ActionResult<
            ApiResponse<object?>>>
        Disable(
            Guid id,
            CancellationToken cancellationToken)
    {
        await service.SetActiveAsync(
            ScopeHotelId,
            CurrentRoleNames,
            CurrentUserId,
            id,
            false,
            cancellationToken);

        return Ok(
            ApiResponse<object?>.Action(
                null,
                "User disabled successfully.",
                "USER_DISABLED",
                HttpContext
                    .TraceIdentifier));
    }

    // =========================================================
    // RESET PASSWORD
    // =========================================================

    [HasPermission(
        Permissions.Administration.UsersManage)]
    [HttpPost(
        "{id:guid}/reset-password")]
    public async Task<
        ActionResult<
            ApiResponse<object?>>>
        ResetPassword(
            Guid id,
            [FromBody]
            ResetUserPasswordRequest request,
            CancellationToken cancellationToken)
    {
        await service
            .ResetPasswordAsync(
                ScopeHotelId,
                CurrentRoleNames,
                CurrentUserId,
                id,
                request.Password,
                cancellationToken);

        return Ok(
            ApiResponse<object?>.Action(
                null,
                "User password reset successfully.",
                "PASSWORD_RESET",
                HttpContext
                    .TraceIdentifier));
    }

    // =========================================================
    // DELETE
    // =========================================================

    [HasPermission(
        Permissions.Administration.UsersManage)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult>
        Delete(
            Guid id,
            CancellationToken cancellationToken)
    {
        await service.DeleteAsync(
            ScopeHotelId,
            CurrentRoleNames,
            CurrentUserId,
            id,
            cancellationToken);

        return NoContent();
    }
}