using HotelManagement.Application.Features.AuditLogs;
using HotelManagement.Domain.Modules.Identity.Constants;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[Authorize]
[ApiController]
[Route("api/v1/audit-logs")]
[Produces("application/json")]
public sealed class AuditLogsController : ControllerBase
{
    private readonly AuditLogService _auditLogService;

    public AuditLogsController(
        AuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    // =========================================================
    // GET AUDIT LOGS
    // GET /api/v1/audit-logs
    //
    // Examples:
    //
    // /api/v1/audit-logs
    //
    // /api/v1/audit-logs?pageNumber=1&pageSize=20
    //
    // /api/v1/audit-logs
    //      ?pageNumber=1
    //      &pageSize=20
    //      &search=reservation
    // =========================================================

    [HasPermission(Permissions.Administration.AuditView)]
    [HttpGet]
    [ProducesResponseType(
        typeof(object),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<object>> Get(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        // =====================================================
        // PAGINATION VALIDATION
        // =====================================================

        if (pageNumber < 1)
        {
            return BadRequest(new
            {
                message = "Page number must be greater than 0."
            });
        }

        if (pageSize < 1)
        {
            return BadRequest(new
            {
                message = "Page size must be greater than 0."
            });
        }

        if (pageSize > 100)
        {
            return BadRequest(new
            {
                message = "Page size cannot be greater than 100."
            });
        }

        // =====================================================
        // NORMALIZE SEARCH
        // =====================================================

        search = string.IsNullOrWhiteSpace(search)
            ? null
            : search.Trim();

        // =====================================================
        // LOAD AUDIT LOGS
        // =====================================================

        var result =
            await _auditLogService.GetAsync(
                pageNumber,
                pageSize,
                search,
                cancellationToken);

        return Ok(result);
    }
}