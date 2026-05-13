using Microsoft.Extensions.Logging;

namespace Syncrer.Sync;

public static class FileUtils
{
    public static void DeleteFiles(HashSet<string> files, DirectoryInfo folder, ILogger logger)
    {
        var actionType = SyncActionType.Deleted;
        foreach (var file in files)
        {
            var path = Path.Combine(folder.FullName, file);
            File.Delete(path);
            logger.LogInformation("{actionType} file: {file}", actionType, file);
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
            Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
            File.Copy(sourcePath, targetPath, true);
            logger.LogInformation("{actionType} file: {file}", actionType, file);
        }
    }
}