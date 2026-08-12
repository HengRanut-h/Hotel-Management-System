# API controller inventory

| Controller | Base route | Actions found |
|---|---|---|
| AmenitiesController.cs | `api/v1/amenities` | GET, GET {id:guid}, POST, PUT {id:guid}, DELETE {id:guid} |
| AttendanceController.cs | `api/v1/attendance` | GET, GET {id:guid}, POST, PUT {id:guid}, PATCH {id:guid}/status, DELETE {id:guid} |
| AuditLogsController.cs | `api/v1/audit-logs` | GET |
| AuthController.cs | `api/v1/auth` | POST login, POST refresh-token, POST logout |
| AvailabilityController.cs | `api/v1/availability` | GET |
| BranchesController.cs | `api/v1/branches` | GET, GET {id:guid}, POST, PUT {id:guid}, DELETE {id:guid} |
| BuildingsController.cs | `api/v1/buildings` | GET, GET {id:guid}, POST, PUT {id:guid}, DELETE {id:guid} |
| CheckInsController.cs | `api/v1/check-ins` | POST {reservationId:guid} |
| CheckOutsController.cs | `api/v1/check-outs` | POST {reservationId:guid} |
| ComplaintsController.cs | `api/v1/complaints` | GET, GET {id:guid}, POST, PUT {id:guid}, PATCH {id:guid}/status, DELETE {id:guid} |
| DashboardController.cs | `api/v1/dashboard` | GET |
| DepartmentsController.cs | `api/v1/departments` | GET, GET {id:guid}, POST, PUT {id:guid}, DELETE {id:guid} |
| DepositsController.cs | `api/v1/deposits` | GET, GET {id:guid}, POST, PUT {id:guid}, PATCH {id:guid}/status, DELETE {id:guid} |
| DiscountsController.cs | `api/v1/discounts` | GET, GET {id:guid}, POST, PUT {id:guid}, DELETE {id:guid} |
| EmployeesController.cs | `api/v1/employees` | GET, POST, PUT {id:guid}, DELETE {id:guid} |
| FeatureFlagsController.cs | `api/v1/feature-flags` | GET, GET {id:guid}, POST, PUT {id:guid}, DELETE {id:guid} |
| FloorsController.cs | `api/v1/floors` | GET, GET {id:guid}, POST, PUT {id:guid}, DELETE {id:guid} |
| FoliosController.cs | `api/v1/folios` | GET {id:guid}, POST, POST {id:guid}/charges, POST {id:guid}/close |
| GoodsReceiptsController.cs | `api/v1/goods-receipts` | GET, GET {id:guid}, POST, PUT {id:guid}, PATCH {id:guid}/status, DELETE {id:guid} |
| GuestDocumentsController.cs | `api/v1/guest-documents` | GET, GET {id:guid}, POST, PUT {id:guid}, PATCH {id:guid}/status, DELETE {id:guid} |
| GuestRequestsController.cs | `api/v1/guest-requests` | GET, GET {id:guid}, POST, PUT {id:guid}, PATCH {id:guid}/status, DELETE {id:guid} |
| GuestsController.cs | `api/v1/guests` | GET, GET {id:guid}, POST, PUT {id:guid} |
| HotelsController.cs | `api/v1/hotels` | GET, GET {id:guid}, POST, PUT {id:guid}, DELETE {id:guid} |
| HousekeepingController.cs | `api/v1/housekeeping` | GET, POST, POST {id:guid}/complete |
| InventoryController.cs | `api/v1/inventory` | GET, POST |
| InvoicesController.cs | `api/v1/invoices` | GET, GET {id:guid}, POST, GET {id:guid}/pdf, POST {id:guid}/payments |
| LaundryController.cs | `api/v1/laundry` | GET, GET {id:guid}, POST, PUT {id:guid}, PATCH {id:guid}/status, DELETE {id:guid} |
| LeaveRequestsController.cs | `api/v1/leave-requests` | GET, GET {id:guid}, POST, PUT {id:guid}, PATCH {id:guid}/status, DELETE {id:guid} |
| LostAndFoundController.cs | `api/v1/lost-and-found` | GET, GET {id:guid}, POST, PUT {id:guid}, PATCH {id:guid}/status, DELETE {id:guid} |
| MaintenanceController.cs | `api/v1/maintenance` | GET, POST, POST {id:guid}/complete |
| MasterDataController.cs | `api/v1/master-data` | GET |
| MeterReadingsController.cs | `api/v1/meter-readings` | GET meter/{meterId:guid}, POST |
| NotificationsController.cs | `api/v1/notifications` | GET, PATCH {id:guid}/read |
| PaymentsController.cs | `api/v1/payments` | GET, GET {id:guid} |
| PermissionsController.cs | `api/v1/permissions` | GET |
| PositionsController.cs | `api/v1/positions` | GET, GET {id:guid}, POST, PUT {id:guid}, DELETE {id:guid} |
| ProfileController.cs | `api/v1/profile` | GET |
| PurchaseOrdersController.cs | `api/v1/purchase-orders` | GET, GET {id:guid}, POST, PUT {id:guid}, PATCH {id:guid}/status, DELETE {id:guid} |
| PurchaseRequestsController.cs | `api/v1/purchase-requests` | GET, GET {id:guid}, POST, PUT {id:guid}, PATCH {id:guid}/status, DELETE {id:guid} |
| RatesController.cs | `api/v1/rates` | GET, GET {id:guid}, POST, PUT {id:guid}, DELETE {id:guid} |
| RefundsController.cs | `api/v1/refunds` | GET, GET {id:guid}, POST, PUT {id:guid}, PATCH {id:guid}/status, DELETE {id:guid} |
| ReportsController.cs | `api/v1/reports` | GET revenue, GET occupancy |
| ReservationsController.cs | `api/v1/reservations` | GET, GET {id:guid}, POST, POST {id:guid}/cancel, POST {id:guid}/check-in, POST {id:guid}/check-out |
| RestaurantController.cs | `api/v1/restaurant` | GET menu, POST categories, POST items, POST orders, POST orders/{id:guid}/items |
| RolesController.cs | `api/v1/roles` | GET, GET {id:guid}, POST, PUT {id:guid}, PUT {id:guid}/permissions, DELETE {id:guid} |
| RoomAssignmentsController.cs | `api/v1/room-assignments` | GET, GET {id:guid}, POST, PUT {id:guid}, PATCH {id:guid}/status, DELETE {id:guid} |
| RoomChangesController.cs | `api/v1/room-changes` | GET, GET {id:guid}, POST, PUT {id:guid}, PATCH {id:guid}/status, DELETE {id:guid} |
| RoomInspectionsController.cs | `api/v1/room-inspections` | GET, GET {id:guid}, POST, PUT {id:guid}, PATCH {id:guid}/status, DELETE {id:guid} |
| RoomTypesController.cs | `api/v1/room-types` | GET, GET {id:guid}, POST, PUT {id:guid}, DELETE {id:guid} |
| RoomsController.cs | `api/v1/rooms` | GET, GET {id:guid}, POST, PUT {id:guid}, PATCH {id:guid}/status, DELETE {id:guid} |
| SecurityIncidentsController.cs | `api/v1/security-incidents` | GET, GET {id:guid}, POST, PUT {id:guid}, PATCH {id:guid}/status, DELETE {id:guid} |
| ServicesController.cs | `api/v1/services` | GET, GET {id:guid}, POST, PUT {id:guid}, DELETE {id:guid} |
| SettingsController.cs | `api/v1/settings` | GET, GET {id:guid}, POST, PUT {id:guid}, DELETE {id:guid} |
| ShiftsController.cs | `api/v1/shifts` | GET, GET {id:guid}, POST, PUT {id:guid}, DELETE {id:guid} |
| StayExtensionsController.cs | `api/v1/stay-extensions` | GET, GET {id:guid}, POST, PUT {id:guid}, PATCH {id:guid}/status, DELETE {id:guid} |
| StockTransactionsController.cs | `api/v1/stock-transactions` | GET, GET {id:guid}, POST, PUT {id:guid}, PATCH {id:guid}/status, DELETE {id:guid} |
| SuppliersController.cs | `api/v1/suppliers` | GET, GET {id:guid}, POST, PUT {id:guid}, DELETE {id:guid} |
| TaxesController.cs | `api/v1/taxes` | GET, GET {id:guid}, POST, PUT {id:guid}, DELETE {id:guid} |
| TransportationController.cs | `api/v1/transportation` | GET, GET {id:guid}, POST, PUT {id:guid}, PATCH {id:guid}/status, DELETE {id:guid} |
| UsersController.cs | `api/v1/users` | GET, GET {id:guid}, POST, PUT {id:guid}, POST {id:guid}/enable, POST {id:guid}/disable, POST {id:guid}/reset-password, DELETE {id:guid} |
| UtilitiesController.cs | `api/v1/utilities` | GET meters, POST meters, POST readings |
| UtilityMetersController.cs | `api/v1/utility-meters` | GET, POST |
| UtilityRatesController.cs | `api/v1/utility-rates` | GET, GET {id:guid}, POST, PUT {id:guid}, DELETE {id:guid} |
| WarehousesController.cs | `api/v1/warehouses` | GET, GET {id:guid}, POST, PUT {id:guid}, DELETE {id:guid} |
