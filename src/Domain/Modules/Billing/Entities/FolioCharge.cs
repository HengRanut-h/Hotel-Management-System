using HotelManagement.Domain.Common.Entities;
namespace HotelManagement.Domain.Modules.Billing.Entities;
public sealed class FolioCharge : AuditableEntity
{
    private FolioCharge() { }
    public FolioCharge(Guid folioId,string category,string description,decimal amount){FolioId=folioId;Category=category.Trim();Description=description.Trim();Amount=amount;}
    public Guid FolioId {get;private set;} public string Category {get;private set;}=string.Empty; public string Description {get;private set;}=string.Empty;
    public decimal Amount {get;private set;} public bool IsVoided {get;private set;} public void Void(){IsVoided=true;MarkUpdated();}
}
