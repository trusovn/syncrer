namespace Syncrer.Tests;

internal sealed class TempDirectory : IDisposable
{
    private readonly string _path;

    public TempDirectory()
    {
        _path = Path.Combine(
            Path.GetTempPath(),
            "Syncrer.Tests",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_path);
    }

    public DirectoryInfo DirectoryInfo => new(_path);

    public DirectoryInfo CreateDirectory(string relativePath)
    {
        var fullPath = GetPath(relativePath);
        Directory.CreateDirectory(fullPath);
        return new DirectoryInfo(fullPath);
    }

    public string CreateFile(string relativePath, string contents)
    {
        var fullPath = GetPath(relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        File.WriteAllText(fullPath, contents);
        return fullPath;
    }

    public string GetPath(string relativePath)
    {
        return Path.Combine(_path, relativePath);
    }

    public void Dispose()
    {
        try
        {
            Directory.Delete(_path, recursive: true);
        }
        catch (DirectoryNotFoundException)
        {
        }
    }
}
