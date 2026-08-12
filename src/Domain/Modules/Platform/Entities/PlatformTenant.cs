using HotelManagement.Domain.Common.Entities;
namespace HotelManagement.Domain.Modules.Platform.Entities;
public sealed class PlatformTenant : AuditableEntity
{
    private PlatformTenant(){} public PlatformTenant(string name,string code,string plan){Name=name.Trim();Code=code.Trim().ToUpperInvariant();Plan=plan.Trim();}
    public string Name{get;private set;}=string.Empty; public string Code{get;private set;}=string.Empty; public string Plan{get;private set;}=string.Empty; public bool IsActive{get;private set;}=true;
}
