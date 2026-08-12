# HotelManagement — Full Backend Foundation

ASP.NET Core **.NET 8** backend for a full Hotel Management System using the chosen architecture:

```text
ONE ASP.NET Core project
+
src/Domain
src/Application
src/Infrastructure
src/Persistence
src/Api
src/Worker
```

## Technology

- ASP.NET Core Web API — .NET 8
- MySQL 8+
- Entity Framework Core 8.0.29
- Pomelo.EntityFrameworkCore.MySql 8.0.3
- AutoMapper 13.0.1
- Swagger / Swashbuckle
- JWT access tokens
- Refresh-token rotation
- Roles + dynamic permission policies
- SignalR
- ASP.NET Core rate limiting
- EF Core SaveChanges interceptors
- BackgroundService workers

## Main modules implemented

### Identity / Security
- Login
- Refresh token
- Logout
- Profile
- Users CRUD/lifecycle
- Enable / disable user
- Reset user password
- Roles CRUD
- Assign permissions to roles
- Permission catalog
- JWT claims for hotel / branch / roles / permissions
- Permission authorization policies
- Password hashing
- Security headers
- Correlation IDs
- Request logging
- Auth rate limiting

### Hotel setup
- Hotels
- Branches
- Buildings
- Floors
- Room types
- Rooms
- Amenities
- Rate plans
- Availability search

### Guest / Front Desk
- Guests
- Guest documents
- Reservations
- Check-in
- Check-out
- Room assignments
- Room changes
- Stay extensions
- Reservation cancellation
- No-show processing foundation

### Billing / Finance
- Folios
- Folio charges
- Invoices
- Invoice items
- Payments
- Deposits
- Refunds
- Taxes
- Discounts
- Invoice PDF generator
- Revenue reports

### Utilities
- Utility rates
- Utility meters
- Meter readings
- Water/electricity usage calculation
- Amount calculation
- Utility billing worker foundation

### Operations
- Housekeeping
- Room inspections
- Maintenance
- Guest services
- Guest requests
- Complaints
- Lost & found
- Restaurant menu and orders
- Laundry
- Transportation
- Security incidents

### Inventory / Purchasing
- Inventory items
- Warehouses
- Stock transactions
- Suppliers
- Purchase requests
- Purchase orders
- Goods receipts

### Human Resources
- Employees
- Departments
- Positions
- Shifts
- Attendance
- Leave requests

### Administration / Platform
- Notifications
- Reports
- Audit-log read API
- Feature flags
- System settings
- Platform tenant domain foundation
- Health checks

### SignalR hubs
- `/hubs/notifications`
- `/hubs/front-desk`
- `/hubs/housekeeping`
- `/hubs/maintenance`

### Workers
- ReservationReminderJob
- NoShowProcessingJob
- InvoiceOverdueJob
- UtilityBillingJob
- EmailDeliveryJob
- SmsDeliveryJob
- DailyRevenueJob
- BackupJob

> Email/SMS/payment/file-storage integrations are implemented as local/development adapters. Replace them with your real providers (SMTP, Twilio/local SMS, ABA/KHQR, S3/Cloudflare R2, etc.) for production.

---

# 1. Requirements

```powershell
dotnet --version
```

Use .NET 8 SDK.

Install EF CLI if needed:

```powershell
dotnet tool install --global dotnet-ef --version 8.0.29
```

or update it:

```powershell
dotnet tool update --global dotnet-ef --version 8.0.29
```

You also need MySQL 8+ running locally.

---

# 2. Configure MySQL

Open:

```text
appsettings.json
```

Change:

```json
"DefaultConnection": "Server=localhost;Port=3306;Database=hotel_management_db;User=root;Password=CHANGE_ME;"
```

Example:

```json
"DefaultConnection": "Server=localhost;Port=3306;Database=hotel_management_db;User=root;Password=1234;"
```

You may also use the environment variable `HOTEL_DB` for design-time EF operations.

---

# 3. Change JWT secret

Change the development key in `appsettings.json` before any real deployment.

For production prefer environment variables / secret storage.

---

# 4. Restore and build

```powershell
dotnet restore
dotnet build
```

---

# 5. Create migration

```powershell
.\scripts\Create-Migration.ps1 -Name InitialCreate
```

or:

```powershell
dotnet ef migrations add InitialCreate `
  --output-dir src/Persistence/Migrations
```

---

# 6. Create/update database

```powershell
.\scripts\Update-Database.ps1
```

or:

```powershell
dotnet ef database update
```

---

# 7. Run

```powershell
dotnet run --launch-profile http
```

API:

```text
http://localhost:5174
```

Swagger:

```text
http://localhost:5174/swagger
```

Health:

```text
http://localhost:5174/health
```

---

# 8. Seed administrator

After the migration exists, application startup seeds the base hotel, branch, room types, roles, permissions and administrator.

Default development administrator:

```text
Email:    admin@hotel.local
Password: ChangeMe123!
```

Change this before production.

---

# 9. Swagger JWT login

Call:

```text
POST /api/v1/auth/login
```

Example:

```json
{
  "email": "admin@hotel.local",
  "password": "ChangeMe123!"
}
```

Copy `accessToken`.

Swagger → **Authorize** → enter:

```text
Bearer YOUR_ACCESS_TOKEN
```

---

# 10. CRUD standard

Master-data modules follow:

```text
GET    /api/v1/<feature>
GET    /api/v1/<feature>/{id}
POST   /api/v1/<feature>
PUT    /api/v1/<feature>/{id}
DELETE /api/v1/<feature>/{id}
```

with search/filter/sort/pagination where applicable.

Transactional modules use business workflows instead of unsafe hard-delete operations, for example:

```text
Reservation -> cancel / check-in / check-out
Room        -> status changes
Folio       -> add charge / close
Invoice     -> issue / payment / PDF
Role        -> assign permissions
User        -> enable / disable / reset password
Inventory   -> stock adjustments
```

---

# 11. Security model

```text
Angular
  ↓
HTTPS / CORS / rate limiting
  ↓
JWT authentication
  ↓
Role + permission authorization
  ↓
Hotel / branch scope
  ↓
Application business rules
  ↓
EF Core / MySQL
  ↓
Audit + background operations
```

The Angular frontend may hide menus/buttons, but **ASP.NET Core remains the security authority**.

---

# 12. Project structure

See:

```text
PROJECT_STRUCTURE.txt
```

The production code intentionally remains **one `HotelManagement.csproj`** while folders preserve the architectural boundaries.
