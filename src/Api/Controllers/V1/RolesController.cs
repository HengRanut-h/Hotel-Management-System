using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.Roles;
using HotelManagement.Domain.Modules.Identity.Constants;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/roles")]
public sealed class RolesController(
    RolesService service)
    : ControllerBase
{
    // =========================================================
    // GET ALL
    // =========================================================

    [HasPermission(Permissions.Administration.RolesManage)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<RoleResponse>>>> GetAll(
        CancellationToken cancellationToken)
    {
        var response =
            await service.GetAllAsync(
                cancellationToken);

        return Ok(
            ApiResponse<IEnumerable<RoleResponse>>.Ok(
                response,
                "Roles retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    [HasPermission(Permissions.Administration.RolesManage)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<RoleResponse>>> Get(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response =
            await service.GetAsync(
                id,
                cancellationToken);

        return Ok(
            ApiResponse<RoleResponse>.Ok(
                response,
                "Role retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE
    // =========================================================

    [HasPermission(Permissions.Administration.RolesManage)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<RoleResponse>>> Create(
        RoleRequest request,
        CancellationToken cancellationToken)
    {
        var response =
            await service.CreateAsync(
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(Get),
            new
            {
                id = response.Id
            },
            ApiResponse<RoleResponse>.Created(
                response,
                "Role created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // UPDATE
    // =========================================================

    [HasPermission(Permissions.Administration.RolesManage)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<RoleResponse>>> Update(
        Guid id,
        RoleRequest request,
        CancellationToken cancellationToken)
    {
        var response =
            await service.UpdateAsync(
                id,
                request,
                cancellationToken);

        return Ok(
            ApiResponse<RoleResponse>.Updated(
                response,
                "Role updated successfully.",
                HttpContext.TraceIdentifier));
    }

    // =========================================================
    // SET PERMISSIONS
    // =========================================================

    [HasPermission(Permissions.Administration.RolesManage)]
    [HttpPut("{id:guid}/permissions")]
    public async Task<ActionResult<ApiResponse<object?>>> SetPermissions(
        Guid id,
        SetRolePermissionsRequest request,
        CancellationToken cancellationToken)
    {
        await service.SetPermissionsAsync(
            id,
            request,
            cancellationToken);

        return Ok(
            ApiResponse<object?>.Action(
                null,
                "Role permissions updated successfully.",
                "ROLE_PERMISSIONS_UPDATED",
                HttpContext.TraceIdentifier));
    }

    // =========================================================
    // DELETE
    // =========================================================

    [HasPermission(Permissions.Administration.RolesManage)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await service.DeleteAsync(
            id,
            cancellationToken);

        return NoContent();
    }
}