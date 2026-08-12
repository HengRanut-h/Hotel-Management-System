using HotelManagement.Domain.Common.Entities;
namespace HotelManagement.Domain.Modules.Guests.Entities;
public sealed class GuestPreference : AuditableEntity
{
    private GuestPreference() { }
    public GuestPreference(Guid hotelId, Guid guestId, string key, string value) { HotelId=hotelId; GuestId=guestId; Key=key.Trim(); Value=value.Trim(); }
    public Guid HotelId { get; private set; } public Guid GuestId { get; private set; }
    public string Key { get; private set; }=string.Empty; public string Value { get; private set; }=string.Empty;
}
