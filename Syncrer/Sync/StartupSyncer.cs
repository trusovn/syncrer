using Microsoft.Extensions.Logging;
using Syncrer.Inputs;

namespace Syncrer.Sync;

public class StartupSyncer(InputParams inputParams, KnownModel model, ILogger<StartupSyncer> logger)
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
            logger.LogDebug("Copying {newFile.FullName} to {newPath}", file.FullName, newPath);
        }

        sw.Stop();
        logger.LogTrace("Took {sw.ElapsedMilliseconds} ms to full copy source to target", sw.ElapsedMilliseconds);
    }

    private void DeleteFolder(DirectoryInfo targetFolder)
    {
        if (targetFolder.Exists)
        {
            logger.LogDebug("Deleting {targetFolder.FullName}...", targetFolder.FullName);
            Directory.Delete(targetFolder.FullName, true);
        }

        Directory.CreateDirectory(targetFolder.FullName);
    }
}