namespace HotelManagement.Application.Abstractions.Authorization;

public interface IPermissionService
{
    bool Has(string permission);
}