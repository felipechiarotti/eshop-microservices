using System.Reflection;
using System.Text.Json;

namespace BuildingBlocks.Util;
public static class AssemblyExtensions
{
    public static async Task<string> ReadEmbeddedFileAsync(this Assembly assembly, string fileName)
    {
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(x => x.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));
        if (resourceName == null)
            throw new FileNotFoundException($"Resource '{fileName}' not found in assembly '{assembly.FullName}'.");
        using Stream stream = assembly.GetManifestResourceStream(resourceName)!;
        using StreamReader reader = new(stream);
        return await reader.ReadToEndAsync();
    }

    public static async Task<TObject?> ReadEmbeddedFileAsync<TObject>(this Assembly assembly, string fileName)
    {
        var contentString = await ReadEmbeddedFileAsync(assembly, fileName);
        return JsonSerializer.Deserialize<TObject>(contentString);
    }
}
