using Quartz;
using Syncrer.Inputs;

namespace Syncrer;

public class Scheduler(InputParams inputParams) : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        CheckDestinationFolder(inputParams.Params.TargetFolder);
        SyncFolders();
        return Task.CompletedTask;
    }

    private void SyncFolders()
    {
        var files = inputParams.Params.SourceFolder.GetFiles("*.*", SearchOption.AllDirectories);
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        foreach (var file in files)
        {
            var newFile = file.CopyTo(Path.Combine(inputParams.Params.TargetFolder.FullName, file.Name),
                overwrite: true);
            Console.WriteLine($"Copied {file.Name} -> {newFile}");
        }

        sw.Stop();
        Console.WriteLine($"Took {sw.ElapsedMilliseconds} ms");
    }

    private void CheckDestinationFolder(DirectoryInfo targetFolder)
    {
        if (targetFolder.Exists)
        {
            Directory.Delete(targetFolder.FullName, true);
        }

        Directory.CreateDirectory(targetFolder.FullName);
    }
}