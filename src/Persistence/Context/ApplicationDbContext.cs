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
using HotelManagement.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Persistence.Context;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<Amenity> Amenities => Set<Amenity>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<CheckInRecord> CheckInRecords => Set<CheckInRecord>();
    public DbSet<CheckOutRecord> CheckOutRecords => Set<CheckOutRecord>();
    public DbSet<Complaint> Complaints => Set<Complaint>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Deposit> Deposits => Set<Deposit>();
    public DbSet<Discount> Discounts => Set<Discount>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();
    public DbSet<Floor> Floors => Set<Floor>();
    public DbSet<Folio> Folios => Set<Folio>();
    public DbSet<FolioCharge> FolioCharges => Set<FolioCharge>();
    public DbSet<GoodsReceipt> GoodsReceipts => Set<GoodsReceipt>();
    public DbSet<Guest> Guests => Set<Guest>();
    public DbSet<GuestDocument> GuestDocuments => Set<GuestDocument>();
    public DbSet<GuestPreference> GuestPreferences => Set<GuestPreference>();
    public DbSet<GuestRequest> GuestRequests => Set<GuestRequest>();
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<HotelBranch> HotelBranches => Set<HotelBranch>();
    public DbSet<HotelService> HotelServices => Set<HotelService>();
    public DbSet<HousekeepingTask> HousekeepingTasks => Set<HousekeepingTask>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<LaundryRequest> LaundryRequests => Set<LaundryRequest>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<LostAndFoundItem> LostAndFoundItems => Set<LostAndFoundItem>();
    public DbSet<MaintenanceRequest> MaintenanceRequests => Set<MaintenanceRequest>();
    public DbSet<MenuCategory> MenuCategories => Set<MenuCategory>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<MeterReading> MeterReadings => Set<MeterReading>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<PlatformTenant> PlatformTenants => Set<PlatformTenant>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseRequest> PurchaseRequests => Set<PurchaseRequest>();
    public DbSet<RatePlan> RatePlans => Set<RatePlan>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Refund> Refunds => Set<Refund>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<RestaurantOrder> RestaurantOrders => Set<RestaurantOrder>();
    public DbSet<RestaurantOrderItem> RestaurantOrderItems => Set<RestaurantOrderItem>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<RoomAssignment> RoomAssignments => Set<RoomAssignment>();
    public DbSet<RoomChange> RoomChanges => Set<RoomChange>();
    public DbSet<RoomInspection> RoomInspections => Set<RoomInspection>();
    public DbSet<RoomType> RoomTypes => Set<RoomType>();
    public DbSet<SecurityIncident> SecurityIncidents => Set<SecurityIncident>();
    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<StayExtension> StayExtensions => Set<StayExtension>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<TaxRate> TaxRates => Set<TaxRate>();
    public DbSet<TransportRequest> TransportRequests => Set<TransportRequest>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<UtilityMeter> UtilityMeters => Set<UtilityMeter>();
    public DbSet<UtilityRate> UtilityRates => Set<UtilityRate>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }


}
