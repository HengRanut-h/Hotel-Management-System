using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.Employees;
using HotelManagement.Domain.Modules.HumanResources.Entities;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/employees")]
public sealed class EmployeesController(
    EmployeeService service)
    : ControllerBase
{
    // =========================================================
    // GET ALL
    // =========================================================

    [HasPermission("employees.view")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<Employee>>>> GetAll(
        CancellationToken cancellationToken)
    {
        var response =
            await service.GetAsync(
                cancellationToken);

        return Ok(
            ApiResponse<IEnumerable<Employee>>.Ok(
                response,
                "Employees retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    [HasPermission("employees.view")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<Employee>>> Get(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response =
            await service.GetByIdAsync(
                id,
                cancellationToken);

        return Ok(
            ApiResponse<Employee>.Ok(
                response,
                "Employee retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE
    // =========================================================

    [HasPermission("employees.create")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Employee>>> Create(
        EmployeeRequest request,
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
            ApiResponse<Employee>.Created(
                response,
                "Employee created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // UPDATE
    // =========================================================

    [HasPermission("employees.update")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<Employee>>> Update(
        Guid id,
        EmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var response =
            await service.UpdateAsync(
                id,
                request,
                cancellationToken);

        return Ok(
            ApiResponse<Employee>.Updated(
                response,
                "Employee updated successfully.",
                HttpContext.TraceIdentifier));
    }

    // =========================================================
    // DELETE
    // =========================================================

    [HasPermission("employees.delete")]
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