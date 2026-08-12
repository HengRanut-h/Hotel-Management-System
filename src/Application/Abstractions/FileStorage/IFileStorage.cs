namespace HotelManagement.Application.Abstractions.FileStorage;

public interface IFileStorage
{
    Task<string> SaveAsync(Stream stream, string fileName, CancellationToken ct = default);
    Task<Stream> OpenReadAsync(string key, CancellationToken ct = default);
}