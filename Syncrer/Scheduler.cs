using Quartz;
using Syncrer.Inputs;
using Syncrer.Sync;

namespace Syncrer;

public class Scheduler(InputParams inputParams, KnownModel knownModel) : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine("Scheduler is running");

        var modificationsModel = ModelUtils.BuildModel(inputParams.Params.SourceFolder);
        var newKnownModel = ModelUtils.CreateCopy(modificationsModel);
        modificationsModel.SymmetricExceptWith(knownModel.Model);

        if (modificationsModel.Count == 0)
        {
            Console.WriteLine("Nothing to do");
            return Task.CompletedTask;
        }

        var fileDifferences = ModelUtils.GetUniquePaths(modificationsModel);
        Console.WriteLine("Found {0} modifications", fileDifferences.Count);
        foreach (var record in fileDifferences)
        {
            Console.WriteLine($"{record}");
        }

        DeleteMatching(inputParams.Params.TargetFolder, fileDifferences);
        CopyMatching(inputParams.Params.SourceFolder, inputParams.Params.TargetFolder, fileDifferences);

        knownModel.UpdateModel(newKnownModel);

        return Task.CompletedTask;
    }

    private static void CopyMatching(
        DirectoryInfo sourceFolder, DirectoryInfo targetFolder, HashSet<string> files
    )
    {
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
            Console.WriteLine($"Copied {file} -> {fullPath}");
        }

        sw.Stop();
        Console.WriteLine($"Took {sw.ElapsedMilliseconds} ms to copy files");
    }

    private static void DeleteMatching(DirectoryInfo folder, HashSet<string> files)
    {
        Console.WriteLine("Deleting filed in target folder started files");
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        foreach (var file in files)
        {
            var fullPath = Path.Combine(folder.FullName, file);
            if (!File.Exists(fullPath)) continue;
            File.Delete(fullPath);
            Console.WriteLine($"Deleted {file} -> {fullPath}");
        }

        sw.Stop();
        Console.WriteLine($"Took {sw.ElapsedMilliseconds} ms to delete filed in target folder");
    }
}