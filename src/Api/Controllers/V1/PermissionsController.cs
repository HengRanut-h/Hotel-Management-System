using HotelManagement.Application.Common.Pagination;
using HotelManagement.Application.Features.Permissions;
using HotelManagement.Application.Features.Permissions.Contracts;
using HotelManagement.Domain.Modules.Identity.Constants;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/permissions")]
[Produces("application/json")]
public sealed class PermissionsController : ControllerBase
{
    private readonly PermissionsService _service;

    public PermissionsController(
        PermissionsService service)
    {
        _service = service;
    }

    // =========================================================
    // GET ALL
    //
    // SEARCH
    // FILTER
    // SORT
    // PAGINATION
    //
    // GET /api/v1/permissions
    // =========================================================

    [HasPermission(
        Permissions.Administration.RolesManage)]
    [HttpGet]
    [ProducesResponseType(
        typeof(PagedResult<PermissionResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        StatusCodes.Status403Forbidden)]
    public Task<PagedResult<PermissionResponse>> GetAll(
        [FromQuery] PermissionQuery query,
        CancellationToken cancellationToken = default)
    {
        return _service.GetAllAsync(
            query,
            cancellationToken);
    }

    // =========================================================
    // GET BY ID
    //
    // GET /api/v1/permissions/{id}
    // =========================================================

    [HasPermission(
        Permissions.Administration.RolesManage)]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(PermissionResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        StatusCodes.Status403Forbidden)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public Task<PermissionResponse> GetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _service.GetByIdAsync(
            id,
            cancellationToken);
    }

    // =========================================================
    // CREATE
    //
    // POST /api/v1/permissions
    // =========================================================

    [HasPermission(
        Permissions.Administration.RolesManage)]
    [HttpPost]
    [ProducesResponseType(
        typeof(PermissionResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        StatusCodes.Status403Forbidden)]
    [ProducesResponseType(
        StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PermissionResponse>> Create(
        [FromBody] CreatePermissionRequest request,
        CancellationToken cancellationToken = default)
    {
        var permission =
            await _service.CreateAsync(
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = permission.Id
            },
            permission);
    }

    // =========================================================
    // UPDATE
    //
    // PUT /api/v1/permissions/{id}
    // =========================================================

    [HasPermission(
        Permissions.Administration.RolesManage)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(
        typeof(PermissionResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        StatusCodes.Status403Forbidden)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        StatusCodes.Status409Conflict)]
    public Task<PermissionResponse> Update(
        Guid id,
        [FromBody] UpdatePermissionRequest request,
        CancellationToken cancellationToken = default)
    {
        return _service.UpdateAsync(
            id,
            request,
            cancellationToken);
    }

    // =========================================================
    // DELETE
    //
    // DELETE /api/v1/permissions/{id}
    // =========================================================

    [HasPermission(
        Permissions.Administration.RolesManage)]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        StatusCodes.Status403Forbidden)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await _service.DeleteAsync(
            id,
            cancellationToken);

        return NoContent();
    }
}