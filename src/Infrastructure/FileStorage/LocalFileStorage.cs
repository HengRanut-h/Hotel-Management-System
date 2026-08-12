using HotelManagement.Application.Abstractions.FileStorage;

namespace HotelManagement.Infrastructure.FileStorage;

public sealed class LocalFileStorage(IWebHostEnvironment env) : IFileStorage
{
    private string Root => Path.Combine(env.ContentRootPath, "App_Data", "uploads");

    public async Task<string> SaveAsync(Stream stream, string fileName, CancellationToken ct = default)
    {
        Directory.CreateDirectory(Root);
        var safe = $"{Guid.NewGuid():N}{Path.GetExtension(fileName)}";
        var path = Path.Combine(Root, safe);
        await using var f = File.Create(path);
        await stream.CopyToAsync(f, ct);
        return safe;
    }

    public Task<Stream> OpenReadAsync(string key, CancellationToken ct = default) =>
        Task.FromResult<Stream>(File.OpenRead(Path.Combine(Root, Path.GetFileName(key))));
}