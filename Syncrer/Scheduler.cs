using Microsoft.Extensions.Logging;
using Quartz;
using Syncrer.Inputs;
using Syncrer.Sync;
using Syncrer.Sync.Model;

namespace Syncrer;

public class Scheduler(InputParams inputParams, KnownModel knownModel, ILogger<Scheduler> logger) : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        logger.LogDebug("Scheduler is running");

        var (filesDiff, sourceModel) = GetModifications();

        if (filesDiff.Count == 0)
        {
            logger.LogDebug("Nothing to do");
            return Task.CompletedTask;
        }

        LogDifferences(filesDiff);

        ActionsMap actionsMap = new(filesDiff, inputParams);

        FileUtils.DeleteFiles(actionsMap.GetDeleted(), inputParams.Params.TargetFolder, logger);
        FileUtils.CopyFiles(
            actionsMap.GetNew(),
            inputParams.Params.SourceFolder,
            inputParams.Params.TargetFolder,
            SyncActionType.New,
            logger);
        FileUtils.CopyFiles(
            actionsMap.GetModified(),
            inputParams.Params.SourceFolder,
            inputParams.Params.TargetFolder,
            SyncActionType.Modified,
            logger);

        knownModel.UpdateModel(sourceModel);

        return Task.CompletedTask;
    }

    private void LogDifferences(HashSet<string> filesDiff)
    {
        logger.LogDebug("Found {fileDifferences.Count} modifications", filesDiff.Count);
        foreach (var record in filesDiff)
        {
            logger.LogDebug("{record}", record);
        }
    }

    private (HashSet<string> filesDiff, HashSet<FileInfoRecord> sourceModel) GetModifications()
    {
        var modificationsModel = ModelUtils.BuildModel(inputParams.Params.SourceFolder, logger);
        var sourceModel = ModelUtils.CreateCopy(modificationsModel);

        modificationsModel.SymmetricExceptWith(knownModel.Model);

        var filesDiff = ModelUtils.GetUniquePaths(modificationsModel);

        return (filesDiff, sourceModel);
    }
}