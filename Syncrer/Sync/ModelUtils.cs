namespace Syncrer.Sync;

public static class ModelUtils
{
    public static HashSet<FileInfoRecord> BuildModel(DirectoryInfo folder)
    {
        var files = folder.GetFiles("*.*", SearchOption.AllDirectories);
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        var model = new HashSet<FileInfoRecord>(files.Length);
        foreach (var file in files)
        {
            model.Add(new FileInfoRecord(
                Path.GetRelativePath(folder.FullName, file.FullName),
                file.LastWriteTimeUtc.Ticks,
                file.Length)
            );
        }

        sw.Stop();
        Console.WriteLine($"Creating model for {folder.Name} took {sw.ElapsedMilliseconds} ms");

        return model;
    }

    public static HashSet<string> GetUniquePaths(HashSet<FileInfoRecord> model)
    {
        HashSet<string> result = new(model.Count);
        foreach (var file in model)
        {
            result.Add(file.RelativePath);
        }
        return result;
    }

    public static HashSet<FileInfoRecord> CreateCopy(HashSet<FileInfoRecord> model)
    {
        return [..model];
    }
}