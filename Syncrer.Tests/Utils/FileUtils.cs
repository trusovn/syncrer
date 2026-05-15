namespace Syncrer.Tests.Utils;

public static class FileUtils
{
    public static void MatchLastWriteTime(string sourcePath, string targetPath)
    {
        File.SetLastWriteTimeUtc(targetPath, File.GetLastWriteTimeUtc(sourcePath));
    }
}