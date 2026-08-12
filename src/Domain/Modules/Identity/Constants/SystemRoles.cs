namespace HotelManagement.Domain.Modules.Identity.Constants;

public static class SystemRoles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string HotelAdmin = "HotelAdmin";
    public const string Manager = "Manager";
    public const string Receptionist = "Receptionist";
    public const string Accountant = "Accountant";
    public const string Housekeeper = "Housekeeper";
    public const string Technician = "Technician";
    public const string Guest = "Guest";

    public static readonly string[] All =
    [
        SuperAdmin, HotelAdmin, Manager, Receptionist,
        Accountant, Housekeeper, Technician, Guest
    ];
}
