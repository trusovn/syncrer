using Microsoft.Extensions.Logging;

namespace Syncrer;

public static class FileUtils
{
    public static void CopyMatching(
        DirectoryInfo sourceFolder, DirectoryInfo targetFolder, HashSet<string> files, ILogger logger
    )
    {
        logger.LogDebug("Copying files to target folder started");
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        foreach (var file in files)
        {
            var fullPath = Path.Combine(sourceFolder.FullName, file);
            if (!File.Exists(fullPath)) continue;
            File.Copy(
                fullPath,
                Path.Combine(targetFolder.FullName, file),
                true
            );
            logger.LogInformation("Copied {file} -> {fullPath}", file, fullPath);
        }

        sw.Stop();
        logger.LogTrace("Took {sw.ElapsedMilliseconds} ms to copy files", sw.ElapsedMilliseconds);
    }

    public static void DeleteMatching(DirectoryInfo folder, HashSet<string> files, ILogger logger)
    {
        logger.LogDebug("Deleting files in target folder started");
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        foreach (var file in files)
        {
            var fullPath = Path.Combine(folder.FullName, file);
            if (!File.Exists(fullPath)) continue;
            File.Delete(fullPath);
            logger.LogInformation("Deleted {fullPath}", fullPath);
        }

        sw.Stop();
        logger.LogTrace("Took {sw.ElapsedMilliseconds} ms to delete filed in target folder", sw.ElapsedMilliseconds);
    }

    public static void DeleteFiles(HashSet<string> files, DirectoryInfo folder, ILogger logger)
    {
        foreach (var file in files)
        {
            var path = Path.Combine(folder.FullName, file);
            File.Delete(path);
            logger.LogInformation("Deleted file {file}", file);
        }
    }

    public static void CopyFiles(
        HashSet<string> files,
        DirectoryInfo sourceFolder,
        DirectoryInfo targetFolder,
        SyncActionType actionType,
        ILogger logger
    )
    {
        foreach (var file in files)
        {
            var sourcePath = Path.Combine(sourceFolder.FullName, file);
            var targetPath = Path.Combine(targetFolder.FullName, file);
            File.Copy(sourcePath, targetPath, true);
            logger.LogInformation("Copied {actionType} file: {file}", actionType, file);
        }
    }
}