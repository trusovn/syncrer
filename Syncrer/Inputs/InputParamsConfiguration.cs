using System.CommandLine;
using System.CommandLine.Help;

namespace Syncrer.Inputs;

public static class InputParamsConfiguration
{
    public static InputParamsRecord ParseParams(string[] args)
    {
        var rootCommand = CreateRootCommand(
            out var sourceFolderOption,
            out var targetFolderOption,
            out var syncIntervalOption);

        var parseResult = rootCommand.Parse(args);
        if (parseResult.Action is HelpAction)
        {
            throw InputParamsException.HelpRequested();
        }

        if (parseResult.Errors.Count <= 0
            && parseResult.GetValue(sourceFolderOption) is { } sourceFolder
            && parseResult.GetValue(targetFolderOption) is { } targetFolder
            && parseResult.GetValue(syncIntervalOption) is var syncInterval)
        {
            return new InputParamsRecord(
                SourceFolder: sourceFolder,
                TargetFolder: targetFolder,
                SyncInterval: syncInterval);
        }

        throw InputParamsException.Invalid(parseResult.Errors.Select(e => e.Message));
    }

    private static RootCommand CreateRootCommand(
        out Option<DirectoryInfo> sourceFolderOption,
        out Option<DirectoryInfo> targetFolderOption,
        out Option<int> syncIntervalOption)
    {
        RootCommand rootCommand = new("Syncrer app that synchronizes two folders (one way)");
        sourceFolderOption = new Option<DirectoryInfo>("--source-folder")
        {
            Description = "The path to the source folder.",
            Required = true,
        };
        rootCommand.Options.Add(sourceFolderOption);

        targetFolderOption = new Option<DirectoryInfo>("--target-folder")
        {
            Description = "The path to the target folder.",
            Required = true,
        };
        rootCommand.Options.Add(targetFolderOption);

        syncIntervalOption = new Option<int>("--sync-interval")
        {
            Description = "Synchronisation interval in seconds.",
            Required = true,
        };
        rootCommand.Options.Add(syncIntervalOption);

        return rootCommand;
    }
}