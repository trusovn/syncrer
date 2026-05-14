using Microsoft.Extensions.Logging;

namespace Syncrer.Sync;

public static class FileUtils
{
    public static void DeleteFiles(HashSet<string> files, DirectoryInfo folder, ILogger logger)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var actionType = SyncActionType.Deleted;
        foreach (var file in files)
        {
            var path = Path.Combine(folder.FullName, file);
            try
            {
                File.Delete(path);
            }
            catch (IOException exception)
            {
                logger.LogError(
                    exception,
                    "{ActionType} file: Failed deleting {File}",
                    actionType,
                    file);
                continue;
            }

            logger.LogInformation("{ActionType} file: {File}", actionType, file);
        }

        sw.Stop();
        logger.LogInformation("{ActionType} files: {Files}; took {Elapsed}", actionType, files.Count, sw.Elapsed);
    }

    public static void CopyFiles(
        HashSet<string> files,
        DirectoryInfo sourceFolder,
        DirectoryInfo targetFolder,
        SyncActionType actionType,
        ILogger logger
    )
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        foreach (var file in files)
        {
            var sourcePath = Path.Combine(sourceFolder.FullName, file);
            var targetPath = Path.Combine(targetFolder.FullName, file);
            Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
            try
            {
                File.Copy(sourcePath, targetPath, true);
            }
            catch (IOException exception)
            {
                logger.LogError(
                    exception,
                    "{ActionType} file: Error copying {File}",
                    actionType,
                    file);
                continue;
            }

            logger.LogInformation("{ActionType} file: {File}", actionType, file);
        }

        sw.Stop();
        logger.LogInformation("{ActionType} files: {Files}; took {Elapsed}", actionType, files.Count, sw.Elapsed);
    }
}