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
    KnownModel knownModel,
    ILogger<SyncExecutor> logger,
    IConfigurationRoot configuration) : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        logger.LogDebug("Scheduler is running");

        var sourceModel = ModelUtils.BuildModel(inputParams.Params.SourceFolder, logger);
        var filesDiff = GetModifications(sourceModel);
        if (filesDiff.Count == 0)
        {
            logger.LogDebug("Nothing to do");
            return Task.CompletedTask;
        }

        logger.LogDebug("Found {fileDifferences.Count} modifications", filesDiff.Count);

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

    private HashSet<string> GetModifications(HashSet<FileInfoRecord> sourceModel)
    {
        var modificationsModel = ModelUtils.CreateCopy(sourceModel);
        modificationsModel.SymmetricExceptWith(knownModel.Model);
        var filesDiff = ModelUtils.GetUniquePaths(modificationsModel);
        FilterOutIgnored(filesDiff);

        return filesDiff;
    }

    private void FilterOutIgnored(HashSet<string> files)
    {
        string[] ignorePatterns = configuration.GetSection("fileIgnorePatterns").Get<string[]>() ?? [];
        var regex = BuildRegex(ignorePatterns);
        files.RemoveWhere(f => regex.IsMatch(Path.GetFileName(f)));
    }

    private static Regex BuildRegex(string[] patterns)
    {
        var joined = string.Join("|", patterns);
        var escaped = joined.Replace("*", ".*").Replace(".", "\\.");
        return new Regex($"^(?:{escaped})$", RegexOptions.Compiled);
    }
}