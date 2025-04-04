using System.Reflection;

namespace BuildingBlocks.Util;
public static class AssemblyExtensions
{

    public static async Task<string> ReadEmbeddedFileAsync(this Assembly assembly, string fileName)
    {
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(x => x.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));
        if (resourceName == null)
            throw new FileNotFoundException($"Resource '{fileName}' not found in assembly '{assembly.FullName}'.");
        using Stream stream = assembly.GetManifestResourceStream(resourceName);
        using StreamReader reader = new(stream);
        return await reader.ReadToEndAsync();
    }
}
