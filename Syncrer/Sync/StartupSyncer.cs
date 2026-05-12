using Syncrer.Inputs;

namespace Syncrer.Sync;

public class StartupSyncer(InputParams inputParams, KnownModel model)
{
    public void Run()
    {
        DeleteFolder(inputParams.Params.TargetFolder);
        CopyFolders(inputParams.Params.SourceFolder, inputParams.Params.TargetFolder);
        model.BuildNew(inputParams.Params.SourceFolder);
    }

    private void CopyFolders(DirectoryInfo sourceFolder, DirectoryInfo targetFolder)
    {
        var files = sourceFolder.GetFiles("*.*", SearchOption.AllDirectories);
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        foreach (var file in files)
        {
            var newPath = file.FullName.Replace(sourceFolder.FullName, targetFolder.FullName);
            Directory.CreateDirectory(Path.GetDirectoryName(newPath));
            var newFile = file.CopyTo(newPath, true);
            Console.WriteLine($"Copied {file.Name} -> {newFile}");
        }

        sw.Stop();
        Console.WriteLine($"Took {sw.ElapsedMilliseconds} ms to full copy source to target");
    }

    private static void DeleteFolder(DirectoryInfo targetFolder)
    {
        if (targetFolder.Exists)
        {
            Console.WriteLine($"Deleting {targetFolder.FullName}...");
            Directory.Delete(targetFolder.FullName, true);
        }

        Directory.CreateDirectory(targetFolder.FullName);
    }
}