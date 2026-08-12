using AutoMapper;

using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.Authentication.Contracts;
using HotelManagement.Application.Features.Availability;
using HotelManagement.Application.Features.Guests.Contracts;
using HotelManagement.Application.Features.Hotels;
using HotelManagement.Application.Features.Invoices.Contracts;
using HotelManagement.Application.Features.Permissions.Contracts;
using HotelManagement.Application.Features.Reservations.Contracts;
using HotelManagement.Application.Features.Roles;
using HotelManagement.Application.Features.Rooms.Contracts;
using HotelManagement.Application.Features.RoomTypes;
using HotelManagement.Application.Features.Users.Contracts;
using HotelManagement.Application.Features.Utilities.Contracts;

using HotelManagement.Domain.Common.Interfaces;

using HotelManagement.Domain.Modules.Administration.Entities;
using HotelManagement.Domain.Modules.Billing.Entities;
using HotelManagement.Domain.Modules.FrontDesk.Entities;
using HotelManagement.Domain.Modules.Guests.Entities;
using HotelManagement.Domain.Modules.Hotels.Entities;
using HotelManagement.Domain.Modules.Housekeeping.Entities;
using HotelManagement.Domain.Modules.HumanResources.Entities;
using HotelManagement.Domain.Modules.Identity.Entities;
using HotelManagement.Domain.Modules.Inventory.Entities;
using HotelManagement.Domain.Modules.Laundry.Entities;
using HotelManagement.Domain.Modules.Purchasing.Entities;
using HotelManagement.Domain.Modules.Rates.Entities;
using HotelManagement.Domain.Modules.Reservations.Entities;
using HotelManagement.Domain.Modules.Rooms.Entities;
using HotelManagement.Domain.Modules.Security.Entities;
using HotelManagement.Domain.Modules.Services.Entities;
using HotelManagement.Domain.Modules.Transportation.Entities;
using HotelManagement.Domain.Modules.Utilities.Entities;

namespace HotelManagement.Application.Mapping;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        // =====================================================
        // IDENTITY
        // =====================================================

        CreateMap<User, UserResponse>()
            .ForMember(
                destination => destination.Roles,
                options => options.MapFrom(
                    source => source.UserRoles
                        .Select(x => x.Role.Name)
                        .OrderBy(x => x)
                        .ToList()));

        CreateMap<User, UserSummaryResponse>()
            .ForMember(
                destination => destination.Roles,
                options => options.MapFrom(
                    source => source.UserRoles
                        .Select(x => x.Role.Name)
                        .OrderBy(x => x)
                        .ToList()))
            .ForMember(
                destination => destination.Permissions,
                options => options.MapFrom(
                    source => source.UserRoles
                        .SelectMany(
                            x => x.Role.RolePermissions)
                        .Select(
                            x => x.Permission.Name)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList()));

        CreateMap<Role, RoleResponse>()
            .ConstructUsing(
                source => new RoleResponse(
                    source.Id,
                    source.Name,
                    source.RolePermissions
                        .Select(x => x.Permission.Name)
                        .OrderBy(x => x)
                        .ToList(),
                    source.UserRoles.Count));

        // =====================================================
        // PERMISSIONS
        // =====================================================

        CreateMap<Permission, PermissionResponse>()
            .ForMember(
                destination => destination.RoleCount,
                options => options.MapFrom(
                    source =>
                        source.RolePermissions.Count));

        // =====================================================
        // HOTELS
        // =====================================================

        CreateMap<Hotel, HotelResponse>();

        // =====================================================
        // ROOM TYPES
        // =====================================================

        CreateMap<RoomType, RoomTypeResponse>();

        // =====================================================
        // ROOMS
        // =====================================================

        CreateMap<Room, RoomResponse>()
            .ForMember(
                destination => destination.RoomTypeName,
                options => options.MapFrom(
                    source =>
                        source.RoomType.Name))
            .ForMember(
                destination => destination.Status,
                options => options.MapFrom(
                    source =>
                        source.Status.ToString()));

        // =====================================================
        // AVAILABILITY
        // =====================================================

        CreateMap<Room, AvailabilityRoom>()
            .ConstructUsing(
                source =>
                    new AvailabilityRoom(
                        source.Id,
                        source.RoomNumber,
                        source.RoomTypeId,
                        source.RoomType.Name,
                        source.RoomType.BaseRate,
                        source.Status.ToString()));

        // =====================================================
        // GUESTS
        // =====================================================

        CreateMap<Guest, GuestResponse>();

        // =====================================================
        // RESERVATIONS
        // =====================================================

        CreateMap<Reservation, ReservationResponse>()
            .ForMember(
                destination => destination.GuestName,
                options => options.MapFrom(
                    source =>
                        source.Guest.FullName))
            .ForMember(
                destination => destination.RoomTypeName,
                options => options.MapFrom(
                    source =>
                        source.RoomType.Name))
            .ForMember(
                destination => destination.RoomNumber,
                options => options.MapFrom(
                    source =>
                        source.Room != null
                            ? source.Room.RoomNumber
                            : null))
            .ForMember(
                destination => destination.Nights,
                options => options.MapFrom(
                    source =>
                        source.Nights))
            .ForMember(
                destination => destination.Status,
                options => options.MapFrom(
                    source =>
                        source.Status.ToString()));

        // =====================================================
        // INVOICE ITEMS
        // =====================================================

        CreateMap<InvoiceItem, InvoiceItemResponse>();

        // =====================================================
        // INVOICES
        // =====================================================

        CreateMap<Invoice, InvoiceResponse>()
            .ForMember(
                destination => destination.GuestName,
                options => options.MapFrom(
                    source =>
                        source.Guest.FullName))
            .ForMember(
                destination => destination.BalanceAmount,
                options => options.MapFrom(
                    source =>
                        source.BalanceAmount))
            .ForMember(
                destination => destination.Status,
                options => options.MapFrom(
                    source =>
                        source.Status.ToString()));

        // =====================================================
        // UTILITY METERS
        // =====================================================

        CreateMap<UtilityMeter, UtilityMeterResponse>()
            .ForMember(
                destination => destination.RoomNumber,
                options => options.MapFrom(
                    source =>
                        source.Room.RoomNumber))
            .ForMember(
                destination => destination.Type,
                options => options.MapFrom(
                    source =>
                        source.Type.ToString()));

        // =====================================================
        // METER READINGS
        // =====================================================

        CreateMap<MeterReading, MeterReadingResponse>();

        // =====================================================
        // SHARED CATALOG MODULES
        // =====================================================

        CreateCatalogMap<HotelBranch>();
        CreateCatalogMap<Building>();
        CreateCatalogMap<Floor>();

        CreateCatalogMap<RoomType>();
        CreateCatalogMap<Amenity>();

        CreateCatalogMap<RatePlan>();

        CreateCatalogMap<TaxRate>();
        CreateCatalogMap<Discount>();

        CreateCatalogMap<UtilityRate>();

        CreateCatalogMap<HotelService>();

        CreateCatalogMap<Warehouse>();

        CreateCatalogMap<Supplier>();

        CreateCatalogMap<Department>();
        CreateCatalogMap<Position>();
        CreateCatalogMap<Shift>();

        CreateCatalogMap<FeatureFlag>();
        CreateCatalogMap<SystemSetting>();

        // =====================================================
        // SHARED OPERATIONAL MODULES
        // =====================================================

        // Front Desk
        CreateOperationalMap<RoomAssignment>();
        CreateOperationalMap<RoomChange>();
        CreateOperationalMap<StayExtension>();

        // Guest documents
        CreateOperationalMap<GuestDocument>();

        // Billing
        CreateOperationalMap<Deposit>();
        CreateOperationalMap<Refund>();

        // Housekeeping
        CreateOperationalMap<RoomInspection>();

        // Guest services
        CreateOperationalMap<GuestRequest>();
        CreateOperationalMap<Complaint>();
        CreateOperationalMap<LostAndFoundItem>();

        // Laundry
        CreateOperationalMap<LaundryRequest>();

        // Inventory
        CreateOperationalMap<StockTransaction>();

        // Purchasing
        CreateOperationalMap<PurchaseRequest>();
        CreateOperationalMap<PurchaseOrder>();
        CreateOperationalMap<GoodsReceipt>();

        // Human Resources
        CreateOperationalMap<AttendanceRecord>();
        CreateOperationalMap<LeaveRequest>();

        // Security
        CreateOperationalMap<SecurityIncident>();

        // Transportation
        CreateOperationalMap<TransportRequest>();
    }

    // =========================================================
    // CATALOG MAPPING
    //
    // Used by:
    //
    // Branches
    // Buildings
    // Floors
    // Amenities
    // Rates
    // Taxes
    // Discounts
    // Utility Rates
    // Services
    // Warehouses
    // Suppliers
    // Departments
    // Positions
    // Shifts
    // Feature Flags
    // Settings
    // =========================================================

    private void CreateCatalogMap<TEntity>()
        where TEntity : class, ICatalogEntity
    {
        CreateMap<TEntity, CatalogResponse>()
            .ConstructUsing(
                source =>
                    new CatalogResponse(
                        source.Id,
                        source.HotelId,
                        source.BranchId,
                        source.Name,
                        source.Code,
                        source.Description,
                        source.IsActive,
                        source.CreatedAtUtc));
    }

    // =========================================================
    // OPERATIONAL MAPPING
    //
    // Used by:
    //
    // Deposits
    // Refunds
    // Guest Documents
    // Room Assignments
    // Room Changes
    // Stay Extensions
    // Room Inspections
    // Guest Requests
    // Complaints
    // Lost And Found
    // Laundry
    // Stock Transactions
    // Purchase Requests
    // Purchase Orders
    // Goods Receipts
    // Attendance
    // Leave Requests
    // Security Incidents
    // Transportation
    // =========================================================

    private void CreateOperationalMap<TEntity>()
        where TEntity : class, IOperationalRecord
    {
        CreateMap<TEntity, OperationalResponse>()
            .ConstructUsing(
                source =>
                    new OperationalResponse(
                        source.Id,
                        source.HotelId,
                        source.BranchId,
                        source.ReferenceNumber,
                        source.Title,
                        source.Status,
                        source.Notes,
                        source.Amount,
                        source.EventAtUtc,
                        source.RelatedEntityId,
                        source.RelatedEntityType,
                        source.CreatedAtUtc));
    }
}