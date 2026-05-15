using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using Syncrer.Inputs;
using Syncrer.Sync;
using Syncrer.Sync.Model;

namespace Syncrer;

[DisallowConcurrentExecution]
public class SyncExecutor(
    InputParams inputParams,
    KnownModelStore knownModelStore,
    ILogger<SyncExecutor> logger,
    IConfigurationRoot configuration) : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        logger.LogDebug("Scheduler is running");

        FolderSnapshot sourceSnapshot;
        try
        {
            sourceSnapshot = ModelUtils.BuildModel(inputParams.Params.SourceFolder, logger);
        }
        catch (IOException exception)
        {
            logger.LogCritical(exception, "Failure during creating model on source folder");
            return Task.CompletedTask;
        }

        HashSet<string> filesDiff = GetModifications(sourceSnapshot);
        if (filesDiff.Count == 0)
        {
            logger.LogDebug("Nothing to do");
            return RebuildKnownModelAndComplete();
        }

        logger.LogDebug("Found {FileDifferencesCount} modifications", filesDiff.Count);

        ActionsMap actionsMap = new(filesDiff, inputParams);

        ExecuteSync(actionsMap);

        return RebuildKnownModelAndComplete();
    }

    private Task RebuildKnownModelAndComplete()
    {
        // TODO: do the rebuild in a separate 'check for health' process - not every time here
        knownModelStore.BuildNew(inputParams.Params.TargetFolder);
        return Task.CompletedTask;
    }

    private void ExecuteSync(ActionsMap actionsMap)
    {
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
    }

    private HashSet<string> GetModifications(FolderSnapshot sourceSnapshot)
    {
        FolderSnapshot modificationsModel = ModelUtils.CreateCopy(sourceSnapshot);
        modificationsModel.SymmetricExceptWith(knownModelStore.FolderSnapshot);
        var filesDiff = ModelUtils.GetUniquePaths(modificationsModel);
        FilterOutIgnored(filesDiff);

        return filesDiff;
    }

    private void FilterOutIgnored(HashSet<string> files)
    {
        string[] ignorePatterns = configuration.GetSection("fileIgnorePatterns").Get<string[]>() ?? [];
        var regex = BuildRegex(ignorePatterns);
        files.RemoveWhere(f =>
            f.Split(Path.DirectorySeparatorChar)
                .Any(regex.IsMatch));
    }

    private static Regex BuildRegex(string[] patterns)
    {
        var joined = string.Join("|", patterns);
        var escaped = joined.Replace(".", "\\.").Replace("*", ".*");
        return new Regex($"^(?:{escaped})$", RegexOptions.Compiled);
    }
}