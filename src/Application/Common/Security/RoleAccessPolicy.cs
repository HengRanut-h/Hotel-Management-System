namespace HotelManagement.Application.Common.Security;

// =========================================================
// ROLE ACCESS POLICY
//
// Smaller Level = More Authority
//
// L0  = Platform
// L10 = Hotel Administration
// L20 = Managers
// L30 = Operational Staff
// L40 = Guest / Customer
//
// This policy controls:
//
// - Who appears in Users list
// - Who can be viewed
// - Who can be edited
// - Who can be enabled/disabled
// - Who can have password reset
// - Who can be deleted
// - Which roles can be assigned
//
// Permission checks still happen separately.
//
// Final authorization:
//
// Permission
// + Hotel scope
// + Role level
// + Portal/domain scope
// =========================================================

public static class RoleAccessPolicy
{
    // =====================================================
    // DOMAINS / PORTALS
    // =====================================================

    public static class Domains
    {
        public const string Platform =
            "platform";

        public const string All =
            "all";

        public const string General =
            "general";

        public const string Administration =
            "administration";

        public const string HumanResources =
            "human-resources";

        public const string FrontDesk =
            "front-desk";

        public const string Finance =
            "finance";

        public const string Housekeeping =
            "housekeeping";

        public const string Maintenance =
            "maintenance";

        public const string Restaurant =
            "restaurant";

        public const string Inventory =
            "inventory";

        public const string Purchasing =
            "purchasing";

        public const string Laundry =
            "laundry";

        public const string Security =
            "security";

        public const string Transportation =
            "transportation";

        public const string GuestServices =
            "guest-services";

        public const string Guest =
            "guest";

        public const string Custom =
            "custom";

        public const string Unassigned =
            "unassigned";
    }

    // =====================================================
    // LEVELS
    // =====================================================

    public static class Levels
    {
        public const int SuperAdmin =
            0;

        public const int HotelAdmin =
            10;

        public const int Manager =
            20;

        public const int Staff =
            30;

        public const int Guest =
            40;

        public const int Unassigned =
            50;
    }

    // =====================================================
    // PROFILE
    // =====================================================

    private sealed record RoleProfile(
        int Level,
        string Domain,
        bool ManagesAllDomains = false);

    // =====================================================
    // ROLE MAP
    //
    // Existing roles + recommended portal roles.
    // =====================================================

    private static readonly
        Dictionary<string, RoleProfile>
        Profiles =
            new(
                StringComparer.OrdinalIgnoreCase)
            {
                // =========================================
                // PLATFORM
                // =========================================

                ["SuperAdmin"] =
                    new(
                        Levels.SuperAdmin,
                        Domains.Platform,
                        true),

                // =========================================
                // HOTEL ADMINISTRATION
                // =========================================

                ["HotelAdmin"] =
                    new(
                        Levels.HotelAdmin,
                        Domains.Administration,
                        true),

                // =========================================
                // GENERAL MANAGEMENT
                // =========================================

                ["Manager"] =
                    new(
                        Levels.Manager,
                        Domains.General,
                        true),

                // =========================================
                // HR
                // =========================================

                ["HRManager"] =
                    new(
                        Levels.Manager,
                        Domains.HumanResources),

                ["HRStaff"] =
                    new(
                        Levels.Staff,
                        Domains.HumanResources),

                ["Recruiter"] =
                    new(
                        Levels.Staff,
                        Domains.HumanResources),

                ["PayrollOfficer"] =
                    new(
                        Levels.Staff,
                        Domains.HumanResources),

                // =========================================
                // FRONT DESK
                // =========================================

                ["FrontDeskManager"] =
                    new(
                        Levels.Manager,
                        Domains.FrontDesk),

                ["Receptionist"] =
                    new(
                        Levels.Staff,
                        Domains.FrontDesk),

                ["ReservationAgent"] =
                    new(
                        Levels.Staff,
                        Domains.FrontDesk),

                // =========================================
                // FINANCE
                // =========================================

                ["FinanceManager"] =
                    new(
                        Levels.Manager,
                        Domains.Finance),

                ["Accountant"] =
                    new(
                        Levels.Staff,
                        Domains.Finance),

                ["Cashier"] =
                    new(
                        Levels.Staff,
                        Domains.Finance),

                // =========================================
                // HOUSEKEEPING
                // =========================================

                ["HousekeepingManager"] =
                    new(
                        Levels.Manager,
                        Domains.Housekeeping),

                ["Housekeeper"] =
                    new(
                        Levels.Staff,
                        Domains.Housekeeping),

                ["Inspector"] =
                    new(
                        Levels.Staff,
                        Domains.Housekeeping),

                // =========================================
                // MAINTENANCE
                // =========================================

                ["MaintenanceManager"] =
                    new(
                        Levels.Manager,
                        Domains.Maintenance),

                ["Technician"] =
                    new(
                        Levels.Staff,
                        Domains.Maintenance),

                // =========================================
                // RESTAURANT
                // =========================================

                ["RestaurantManager"] =
                    new(
                        Levels.Manager,
                        Domains.Restaurant),

                ["Waiter"] =
                    new(
                        Levels.Staff,
                        Domains.Restaurant),

                ["RestaurantCashier"] =
                    new(
                        Levels.Staff,
                        Domains.Restaurant),

                ["KitchenStaff"] =
                    new(
                        Levels.Staff,
                        Domains.Restaurant),

                // =========================================
                // INVENTORY
                // =========================================

                ["InventoryManager"] =
                    new(
                        Levels.Manager,
                        Domains.Inventory),

                ["Storekeeper"] =
                    new(
                        Levels.Staff,
                        Domains.Inventory),

                // =========================================
                // PURCHASING
                // =========================================

                ["PurchasingManager"] =
                    new(
                        Levels.Manager,
                        Domains.Purchasing),

                ["PurchasingOfficer"] =
                    new(
                        Levels.Staff,
                        Domains.Purchasing),

                // =========================================
                // LAUNDRY
                // =========================================

                ["LaundryManager"] =
                    new(
                        Levels.Manager,
                        Domains.Laundry),

                ["LaundryStaff"] =
                    new(
                        Levels.Staff,
                        Domains.Laundry),

                // =========================================
                // SECURITY
                // =========================================

                ["SecurityManager"] =
                    new(
                        Levels.Manager,
                        Domains.Security),

                ["SecurityOfficer"] =
                    new(
                        Levels.Staff,
                        Domains.Security),

                // =========================================
                // TRANSPORTATION
                // =========================================

                ["TransportationManager"] =
                    new(
                        Levels.Manager,
                        Domains.Transportation),

                ["Driver"] =
                    new(
                        Levels.Staff,
                        Domains.Transportation),

                // =========================================
                // GUEST SERVICES
                // =========================================

                ["GuestServicesManager"] =
                    new(
                        Levels.Manager,
                        Domains.GuestServices),

                ["GuestServiceAgent"] =
                    new(
                        Levels.Staff,
                        Domains.GuestServices),

                // =========================================
                // GENERIC STAFF
                // =========================================

                ["Staff"] =
                    new(
                        Levels.Staff,
                        Domains.General),

                // =========================================
                // CUSTOMER
                // =========================================

                ["Guest"] =
                    new(
                        Levels.Guest,
                        Domains.Guest)
            };

    // =====================================================
    // SUPER ADMIN
    // =====================================================

    public static bool IsSuperAdmin(
        IEnumerable<string> roles)
    {
        return roles.Any(
            role =>
                Normalize(role)
                    .Equals(
                        "superadmin",
                        StringComparison
                            .OrdinalIgnoreCase));
    }

    // =====================================================
    // CAN MANAGE USER
    // =====================================================

    public static bool CanManageUser(
        IEnumerable<string> actorRoles,
        IEnumerable<string> targetRoles)
    {
        var actor =
            actorRoles
                .Where(
                    role =>
                        !string.IsNullOrWhiteSpace(
                            role))
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToList();

        var target =
            targetRoles
                .Where(
                    role =>
                        !string.IsNullOrWhiteSpace(
                            role))
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToList();

        if (actor.Count == 0)
        {
            return false;
        }

        // SuperAdmin can manage every lower account.
        if (IsSuperAdmin(actor))
        {
            return !target.Any(
                role =>
                    IsSuperAdmin(
                        [role]));
        }

        var actorLevel =
            GetEffectiveLevel(
                actor);

        var targetLevel =
            GetEffectiveLevel(
                target);

        // Same level or higher authority cannot
        // be managed.
        //
        // HotelAdmin 10 -> HotelAdmin 10 ❌
        // HotelAdmin 10 -> SuperAdmin 0 ❌
        // HotelAdmin 10 -> Manager 20 ✅
        if (actorLevel >= targetLevel)
        {
            return false;
        }

        // HotelAdmin / general Manager can manage
        // lower roles from all hotel portals.
        if (CanManageAllDomains(
                actor,
                actorLevel))
        {
            return true;
        }

        var actorDomains =
            GetManagementDomains(
                actor,
                actorLevel);

        if (actorDomains.Count == 0)
        {
            return false;
        }

        var targetDomains =
            GetTargetDomains(
                target);

        if (targetDomains.Count == 0)
        {
            return false;
        }

        // Department manager must only manage
        // users inside its domain.
        //
        // HRManager -> HRStaff ✅
        // HRManager -> Accountant ❌
        return targetDomains.All(
            domain =>
                actorDomains.Contains(
                    domain));
    }

    // =====================================================
    // CAN ASSIGN ROLE(S)
    // =====================================================

    public static bool CanAssignRoles(
        IEnumerable<string> actorRoles,
        IEnumerable<string> requestedRoles)
    {
        var roles =
            requestedRoles
                .Where(
                    role =>
                        !string.IsNullOrWhiteSpace(
                            role))
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToList();

        if (roles.Count == 0)
        {
            return false;
        }

        return CanManageUser(
            actorRoles,
            roles);
    }

    // =====================================================
    // EFFECTIVE LEVEL
    //
    // Multiple roles:
    //
    // HotelAdmin = 10
    // Receptionist = 30
    //
    // Effective = 10
    // =====================================================

    public static int GetEffectiveLevel(
        IEnumerable<string> roles)
    {
        var list =
            roles
                .Where(
                    role =>
                        !string.IsNullOrWhiteSpace(
                            role))
                .ToList();

        if (list.Count == 0)
        {
            return Levels.Unassigned;
        }

        return list
            .Select(
                role =>
                    GetProfile(role)
                        .Level)
            .Min();
    }

    // =====================================================
    // ALL DOMAIN ACCESS
    // =====================================================

    private static bool CanManageAllDomains(
        IEnumerable<string> roles,
        int effectiveLevel)
    {
        return roles
            .Select(GetProfile)
            .Any(
                profile =>
                    profile.Level ==
                    effectiveLevel
                    &&
                    profile
                        .ManagesAllDomains);
    }

    // =====================================================
    // MANAGEMENT DOMAINS
    //
    // Only roles at actor's effective authority
    // level are used.
    //
    // HRManager(20) + Accountant(30)
    // -> management domain = HR only.
    // =====================================================

    private static HashSet<string>
        GetManagementDomains(
            IEnumerable<string> roles,
            int effectiveLevel)
    {
        return roles
            .Select(GetProfile)
            .Where(
                profile =>
                    profile.Level ==
                    effectiveLevel)
            .Select(
                profile =>
                    profile.Domain)
            .ToHashSet(
                StringComparer.OrdinalIgnoreCase);
    }

    // =====================================================
    // TARGET DOMAINS
    // =====================================================

    private static HashSet<string>
        GetTargetDomains(
            IEnumerable<string> roles)
    {
        var list =
            roles.ToList();

        if (list.Count == 0)
        {
            return
            [
                Domains.Unassigned
            ];
        }

        return list
            .Select(GetProfile)
            .Select(
                profile =>
                    profile.Domain)
            .ToHashSet(
                StringComparer.OrdinalIgnoreCase);
    }

    // =====================================================
    // PROFILE
    //
    // Unknown custom roles default to operational level.
    //
    // HotelAdmin / Manager can manage them.
    // Department managers cannot manage unknown custom
    // roles until added to this policy.
    // =====================================================

    private static RoleProfile GetProfile(
        string roleName)
    {
        if (
            Profiles.TryGetValue(
                roleName.Trim(),
                out var profile))
        {
            return profile;
        }

        return new RoleProfile(
            Levels.Staff,
            Domains.Custom);
    }

    // =====================================================
    // NORMALIZE
    // =====================================================

    private static string Normalize(
        string role)
    {
        return role
            .Trim()
            .Replace(
                "-",
                string.Empty)
            .Replace(
                "_",
                string.Empty)
            .Replace(
                " ",
                string.Empty);
    }
}