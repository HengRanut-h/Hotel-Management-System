namespace HotelManagement.Infrastructure.Integrations;

public sealed class IntegrationRegistry
{
    public IReadOnlyList<string> Enabled => ["Email", "Sms", "FileStorage", "ManualPayments"];
}