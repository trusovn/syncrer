using System.CommandLine;

namespace Syncrer.Inputs;

public static class InputParamsUtils
{
    public static InputParamsRecord ParseParams(string[] args)
    {
        RootCommand rootCommand = new("Syncrer app that synchronizes two folders (one way)");
        Option<DirectoryInfo> sourceFolderOption = new("--source-folder")
        {
            Description = "The path to the source folder.",
            Required = true,
        };
        rootCommand.Options.Add(sourceFolderOption);
        Option<DirectoryInfo> targetFolderOption = new("--target-folder")
        {
            Description = "The path to the target folder.",
            Required = true,
        };
        rootCommand.Options.Add(targetFolderOption);
        Option<int> syncIntervalOption = new("--sync-interval")
        {
            Description = "Synchronisation interval in seconds.",
            Required = true,
        };
        rootCommand.Options.Add(syncIntervalOption);

        var parseResult = rootCommand.Parse(args);
        if (parseResult.Errors.Count <= 0
            && parseResult.GetValue(sourceFolderOption) is { } sourceFolder
            && parseResult.GetValue(targetFolderOption) is { } targetFolder
            && parseResult.GetValue(syncIntervalOption) is var syncInterval)
        {
            return new InputParamsRecord(SourceFolder: sourceFolder, TargetFolder: targetFolder,
                SyncInterval: syncInterval);
        }

        throw new ArgumentException(
            string.Join(Environment.NewLine, parseResult.Errors.Select(e => e.Message)));
    }

    public static void VerifyParams(InputParamsRecord inputParamsRecord)
    {
        var sourceFolder = inputParamsRecord.SourceFolder;
        var targetFolder = inputParamsRecord.TargetFolder;

        if (!sourceFolder.Exists)
        {
            throw new ArgumentException(
                $"Source folder does not exist or cannot be accessed: {sourceFolder.FullName}");
        }

        var targetParent = targetFolder.Parent;
        if (targetParent is null || !targetParent.Exists)
        {
            throw new ArgumentException(
                $"Target folder parent does not exist or cannot be accessed: {targetFolder.FullName}");
        }

        if (inputParamsRecord.SyncInterval < 10)
        {
            throw new ArgumentException("Sync interval must be no less than 10 seconds");
        }
    }
}