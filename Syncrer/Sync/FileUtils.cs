using Microsoft.Extensions.Logging;

namespace Syncrer.Sync;

public static partial class FileUtils
{
    public static void DeleteFiles(HashSet<string> files, DirectoryInfo folder, ILogger logger)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        const SyncActionType actionType = SyncActionType.Deleted;
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

            logger.LogActionOnFile(actionType, file);
        }

        sw.Stop();
        logger.LogActionTotalElapsed(actionType, files.Count, sw.Elapsed);
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
            Directory.CreateDirectory(Path.GetDirectoryName(targetPath) ?? throw new InvalidOperationException());
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

            logger.LogActionOnFile(actionType, file);
        }

        sw.Stop();
        logger.LogActionTotalElapsed(actionType, files.Count, sw.Elapsed);
    }

    [LoggerMessage(LogLevel.Information, "{ActionType} file: {File}")]
    static partial void LogActionOnFile(this ILogger logger, SyncActionType actionType, string file);

    [LoggerMessage(LogLevel.Debug, "{ActionType} files: {Files}; took {Elapsed}")]
    static partial void LogActionTotalElapsed(this ILogger logger, SyncActionType actionType, int files, TimeSpan elapsed);
}