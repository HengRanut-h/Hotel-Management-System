using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Domain.Modules.HumanResources.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Employees;

public sealed record EmployeeRequest(
    string EmployeeNumber,
    string FullName,
    string? Email,
    Guid? DepartmentId,
    Guid? PositionId,
    Guid? BranchId);

public sealed class EmployeeService(
    IApplicationDbContext db,
    ICurrentUser currentUser)
{
    // =========================================================
    // HOTEL CONTEXT
    // =========================================================

    private Guid HotelId =>
        currentUser.HotelId
        ?? throw new ForbiddenException(
            "Hotel context required.");

    // =========================================================
    // GET ALL
    // =========================================================

    public Task<List<Employee>> GetAsync(
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        return db.Employees
            .AsNoTracking()
            .Where(
                employee =>
                    employee.HotelId == hotelId
                    &&
                    !employee.IsDeleted)
            .OrderBy(
                employee =>
                    employee.FullName)
            .ToListAsync(
                cancellationToken);
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    public async Task<Employee> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        return await db.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(
                employee =>
                    employee.HotelId == hotelId
                    &&
                    employee.Id == id
                    &&
                    !employee.IsDeleted,
                cancellationToken)
            ?? throw new NotFoundException(
                "Employee not found.");
    }

    // =========================================================
    // CREATE
    // =========================================================

    public async Task<Employee> CreateAsync(
        EmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        var employeeNumber =
            request.EmployeeNumber.Trim();

        var employeeNumberExists =
            await db.Employees.AnyAsync(
                employee =>
                    employee.HotelId == hotelId
                    &&
                    employee.EmployeeNumber == employeeNumber
                    &&
                    !employee.IsDeleted,
                cancellationToken);

        if (employeeNumberExists)
        {
            throw new ConflictException(
                "Employee number already exists.");
        }

        var employee =
            new Employee(
                hotelId,
                request.BranchId
                    ?? currentUser.BranchId,
                employeeNumber,
                request.FullName.Trim(),
                string.IsNullOrWhiteSpace(request.Email)
                    ? null
                    : request.Email.Trim(),
                request.DepartmentId,
                request.PositionId);

        db.Employees.Add(
            employee);

        await db.SaveChangesAsync(
            cancellationToken);

        return employee;
    }

    // =========================================================
    // UPDATE
    // =========================================================

    public async Task<Employee> UpdateAsync(
        Guid id,
        EmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        var employee =
            await db.Employees
                .FirstOrDefaultAsync(
                    employee =>
                        employee.HotelId == hotelId
                        &&
                        employee.Id == id
                        &&
                        !employee.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Employee not found.");

        employee.Update(
            request.FullName.Trim(),
            string.IsNullOrWhiteSpace(request.Email)
                ? null
                : request.Email.Trim(),
            request.DepartmentId,
            request.PositionId);

        await db.SaveChangesAsync(
            cancellationToken);

        return employee;
    }

    // =========================================================
    // SOFT DELETE
    // =========================================================

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        var employee =
            await db.Employees
                .FirstOrDefaultAsync(
                    employee =>
                        employee.HotelId == hotelId
                        &&
                        employee.Id == id
                        &&
                        !employee.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Employee not found.");

        employee.SoftDelete(
            currentUser.UserId);

        await db.SaveChangesAsync(
            cancellationToken);
    }
}