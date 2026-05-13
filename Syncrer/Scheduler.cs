using Microsoft.Extensions.Logging;
using Quartz;
using Syncrer.Inputs;
using Syncrer.Sync;

namespace Syncrer;

public class Scheduler(InputParams inputParams, KnownModel knownModel, ILogger<Scheduler> logger) : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        logger.LogDebug("Scheduler is running");

        var modificationsModel = ModelUtils.BuildModel(inputParams.Params.SourceFolder, logger);
        var newKnownModel = ModelUtils.CreateCopy(modificationsModel);
        modificationsModel.SymmetricExceptWith(knownModel.Model);

        if (modificationsModel.Count == 0)
        {
            logger.LogDebug("Nothing to do");
            return Task.CompletedTask;
        }

        var fileDifferences = ModelUtils.GetUniquePaths(modificationsModel);
        logger.LogDebug("Found {fileDifferences.Count} modifications", fileDifferences.Count);
        foreach (var record in fileDifferences)
        {
            logger.LogDebug("{record}", record);
        }

        DeleteMatching(inputParams.Params.TargetFolder, fileDifferences);
        CopyMatching(inputParams.Params.SourceFolder, inputParams.Params.TargetFolder, fileDifferences);

        knownModel.UpdateModel(newKnownModel);

        return Task.CompletedTask;
    }

    private void CopyMatching(
        DirectoryInfo sourceFolder, DirectoryInfo targetFolder, HashSet<string> files
    )
    {
        logger.LogDebug("Copying files to target folder started");
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
            logger.LogInformation("Copied {file} -> {fullPath}", file, fullPath);
        }

        sw.Stop();
        logger.LogTrace("Took {sw.ElapsedMilliseconds} ms to copy files", sw.ElapsedMilliseconds);
    }

    private void DeleteMatching(DirectoryInfo folder, HashSet<string> files)
    {
        logger.LogDebug("Deleting files in target folder started");
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        foreach (var file in files)
        {
            var fullPath = Path.Combine(folder.FullName, file);
            if (!File.Exists(fullPath)) continue;
            File.Delete(fullPath);
            logger.LogInformation("Deleted {fullPath}", fullPath);
        }

        sw.Stop();
        logger.LogTrace("Took {sw.ElapsedMilliseconds} ms to delete filed in target folder", sw.ElapsedMilliseconds);
    }
}