using HotelManagement.Domain.Common.Entities;
namespace HotelManagement.Domain.Modules.Inventory.Entities;
public sealed class InventoryItem : AuditableEntity
{
    private InventoryItem(){} public InventoryItem(Guid hotelId,string sku,string name,string unit,decimal reorderLevel){HotelId=hotelId;Sku=sku.Trim().ToUpperInvariant();Name=name.Trim();Unit=unit.Trim();ReorderLevel=reorderLevel;}
    public Guid HotelId{get;private set;} public string Sku{get;private set;}=string.Empty; public string Name{get;private set;}=string.Empty; public string Unit{get;private set;}=string.Empty;
    public decimal QuantityOnHand{get;private set;} public decimal ReorderLevel{get;private set;} public void Adjust(decimal quantity){QuantityOnHand+=quantity;MarkUpdated();}
}
