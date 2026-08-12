using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Queries;
using HotelManagement.Application.Common.Models.Requests;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.Attendance;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/attendance")]
public sealed class AttendanceController(AttendanceService service) : ControllerBase
{
    // =========================================================
    // GET ALL
    // =========================================================

    [HasPermission("attendance.view")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<OperationalResponse>>>> GetAll(
        [FromQuery] OperationalQuery request,
        CancellationToken cancellationToken)
    {
        var response = await service.GetAllAsync(
            request,
            cancellationToken);

        var totalPages = (int)Math.Ceiling(
            response.TotalItems / (double)response.PageSize);

        var meta = new
        {
            pagination = new
            {
                response.PageNumber,
                response.PageSize,
                response.TotalItems,
                TotalPages = totalPages,
                HasPreviousPage = response.PageNumber > 1,
                HasNextPage = response.PageNumber < totalPages
            }
        };

        return Ok(
            ApiResponse<IReadOnlyList<OperationalResponse>>.Ok(
                response.Items.ToList(),
                "Attendance records retrieved successfully.",
                meta: meta,
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    [HasPermission("attendance.view")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<OperationalResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await service.GetByIdAsync(
            id,
            cancellationToken);

        return Ok(
            ApiResponse<OperationalResponse>.Ok(
                response,
                "Attendance record retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE
    // =========================================================

    [HasPermission("attendance.create")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<OperationalResponse>>> Create(
        [FromBody] OperationalCreateRequest request,
        CancellationToken cancellationToken)
    {
        var response = await service.CreateAsync(
            request,
            cancellationToken);

        var apiResponse = ApiResponse<OperationalResponse>.Created(
            response,
            "Attendance record created successfully.",
            traceId: HttpContext.TraceIdentifier);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = response.Id
            },
            apiResponse);
    }

    // =========================================================
    // UPDATE
    // =========================================================

    [HasPermission("attendance.update")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<OperationalResponse>>> Update(
        Guid id,
        [FromBody] OperationalUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var response = await service.UpdateAsync(
            id,
            request,
            cancellationToken);

        return Ok(
            ApiResponse<OperationalResponse>.Updated(
                response,
                "Attendance record updated successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CHANGE STATUS
    // =========================================================

    [HasPermission("attendance.manage")]
    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<ApiResponse<OperationalResponse>>> ChangeStatus(
        Guid id,
        [FromBody] ChangeStatusRequest request,
        CancellationToken cancellationToken)
    {
        var response = await service.ChangeStatusAsync(
            id,
            request,
            cancellationToken);

        return Ok(
            ApiResponse<OperationalResponse>.Action(
                response,
                "Attendance status changed successfully.",
                "STATUS_CHANGED",
                HttpContext.TraceIdentifier));
    }

    // =========================================================
    // SOFT DELETE
    // =========================================================

    [HasPermission("attendance.delete")]
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