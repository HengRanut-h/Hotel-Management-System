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
using HotelManagement.Domain.Modules.Maintenance.Entities;
using HotelManagement.Domain.Modules.Notifications.Entities;
using HotelManagement.Domain.Modules.Payments.Entities;
using HotelManagement.Domain.Modules.Platform.Entities;
using HotelManagement.Domain.Modules.Purchasing.Entities;
using HotelManagement.Domain.Modules.Rates.Entities;
using HotelManagement.Domain.Modules.Reservations.Entities;
using HotelManagement.Domain.Modules.Restaurant.Entities;
using HotelManagement.Domain.Modules.Rooms.Entities;
using HotelManagement.Domain.Modules.Security.Entities;
using HotelManagement.Domain.Modules.Services.Entities;
using HotelManagement.Domain.Modules.Transportation.Entities;
using HotelManagement.Domain.Modules.Utilities.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Abstractions.Persistence;

public interface IApplicationDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    DbSet<Amenity> Amenities { get; }
    DbSet<AttendanceRecord> AttendanceRecords { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<Building> Buildings { get; }
    DbSet<CheckInRecord> CheckInRecords { get; }
    DbSet<CheckOutRecord> CheckOutRecords { get; }
    DbSet<Complaint> Complaints { get; }
    DbSet<Department> Departments { get; }
    DbSet<Deposit> Deposits { get; }
    DbSet<Discount> Discounts { get; }
    DbSet<Employee> Employees { get; }
    DbSet<FeatureFlag> FeatureFlags { get; }
    DbSet<Floor> Floors { get; }
    DbSet<Folio> Folios { get; }
    DbSet<FolioCharge> FolioCharges { get; }
    DbSet<GoodsReceipt> GoodsReceipts { get; }
    DbSet<Guest> Guests { get; }
    DbSet<GuestDocument> GuestDocuments { get; }
    DbSet<GuestPreference> GuestPreferences { get; }
    DbSet<GuestRequest> GuestRequests { get; }
    DbSet<Hotel> Hotels { get; }
    DbSet<HotelBranch> HotelBranches { get; }
    DbSet<HotelService> HotelServices { get; }
    DbSet<HousekeepingTask> HousekeepingTasks { get; }
    DbSet<InventoryItem> InventoryItems { get; }
    DbSet<Invoice> Invoices { get; }
    DbSet<InvoiceItem> InvoiceItems { get; }
    DbSet<LaundryRequest> LaundryRequests { get; }
    DbSet<LeaveRequest> LeaveRequests { get; }
    DbSet<LostAndFoundItem> LostAndFoundItems { get; }
    DbSet<MaintenanceRequest> MaintenanceRequests { get; }
    DbSet<MenuCategory> MenuCategories { get; }
    DbSet<MenuItem> MenuItems { get; }
    DbSet<MeterReading> MeterReadings { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<Payment> Payments { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<PlatformTenant> PlatformTenants { get; }
    DbSet<Position> Positions { get; }
    DbSet<PurchaseOrder> PurchaseOrders { get; }
    DbSet<PurchaseRequest> PurchaseRequests { get; }
    DbSet<RatePlan> RatePlans { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Refund> Refunds { get; }
    DbSet<Reservation> Reservations { get; }
    DbSet<RestaurantOrder> RestaurantOrders { get; }
    DbSet<RestaurantOrderItem> RestaurantOrderItems { get; }
    DbSet<Role> Roles { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<Room> Rooms { get; }
    DbSet<RoomAssignment> RoomAssignments { get; }
    DbSet<RoomChange> RoomChanges { get; }
    DbSet<RoomInspection> RoomInspections { get; }
    DbSet<RoomType> RoomTypes { get; }
    DbSet<SecurityIncident> SecurityIncidents { get; }
    DbSet<Shift> Shifts { get; }
    DbSet<StayExtension> StayExtensions { get; }
    DbSet<StockTransaction> StockTransactions { get; }
    DbSet<Supplier> Suppliers { get; }
    DbSet<SystemSetting> SystemSettings { get; }
    DbSet<TaxRate> TaxRates { get; }
    DbSet<TransportRequest> TransportRequests { get; }
    DbSet<User> Users { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<UtilityMeter> UtilityMeters { get; }
    DbSet<UtilityRate> UtilityRates { get; }
    DbSet<Warehouse> Warehouses { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
