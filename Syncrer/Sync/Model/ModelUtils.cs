using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Syncrer.Sync.Model;

public static class ModelUtils
{
    public static FolderSnapshot BuildModel(DirectoryInfo folder, ILogger logger)
    {
        var sw = Stopwatch.StartNew();

        FileInfo[] files = folder.GetFiles("*", SearchOption.AllDirectories);
        var model = new FolderSnapshot(files.Length);
        foreach (var file in files)
        {
            model.Add(new FileInfoRecord(
                Path.GetRelativePath(folder.FullName, file.FullName),
                file.LastWriteTimeUtc.Ticks,
                file.Length)
            );
        }

        sw.Stop();
        logger.LogDebug("Creating model for {FolderName} took {ElapsedMilliseconds} ms", folder.FullName,
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