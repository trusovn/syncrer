using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Syncrer.Sync.Model;

public static class ModelUtils
{
    public static FolderSnapshot BuildModel(
        DirectoryInfo folder,
        ILogger logger,
        int estimatedFileCount = 10000)
    {
        var sw = Stopwatch.StartNew();

        var model = new FolderSnapshot(estimatedFileCount);

        var options = new EnumerationOptions
        {
            RecurseSubdirectories = true,
            IgnoreInaccessible = true
        };

        foreach (FileInfo file in folder.EnumerateFiles("*", options))
        {
            model.Add(new FileInfoRecord(
                Path.GetRelativePath(folder.FullName, file.FullName),
                file.LastWriteTimeUtc.Ticks,
                file.Length
            ));
        }

        logger.LogInformation(
            "Creating model for {FolderName} took {ElapsedMilliseconds} ms",
            folder.FullName,
            sw.ElapsedMilliseconds);

        return model;
    }

    public static HashSet<string> GetUniquePaths(FolderSnapshot snapshot)
    {
        HashSet<string> result = new(snapshot.Count);
        foreach (FileInfoRecord file in snapshot)
        {
            result.Add(file.RelativePath);
        }

        return result;
    }

    public static FolderSnapshot CreateCopy(FolderSnapshot snapshot)
    {
        return snapshot.GetCopy();
    }
}