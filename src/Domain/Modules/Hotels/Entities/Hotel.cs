using HotelManagement.Domain.Common.Entities;
namespace HotelManagement.Domain.Modules.Hotels.Entities;
public sealed class Hotel : AuditableEntity
{
    private Hotel() { }
    public Hotel(string name,string code,string currency="USD"){Name=name.Trim();Code=code.Trim().ToUpperInvariant();Currency=currency.Trim().ToUpperInvariant();}
    public string Name{get;private set;}=string.Empty; public string Code{get;private set;}=string.Empty; public string Currency{get;private set;}="USD"; public bool IsActive{get;private set;}=true;
    public ICollection<HotelBranch> Branches{get;private set;}=new List<HotelBranch>();
    public void Update(string name,string code,string currency){Name=name.Trim();Code=code.Trim().ToUpperInvariant();Currency=currency.Trim().ToUpperInvariant();MarkUpdated();}
    public void SetActive(bool active){IsActive=active;MarkUpdated();}
}
