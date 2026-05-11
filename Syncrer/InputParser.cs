using System.CommandLine;

namespace Syncrer;

public class InputParser
{
    public static InputParams GetParams(string[] args)
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
            return new InputParams(SourceFolder: sourceFolder, TargetFolder: targetFolder, SyncInterval: syncInterval);
        }
        throw new ArgumentException(string.Join(" ", parseResult.Errors));
    }

    public static void VerifyParams(InputParams inputParams)
    {
        if (!inputParams.SourceFolder.Exists)
        {
            throw new ArgumentException("Source folder does not exist");
        }
        if (inputParams.TargetFolder.Exists)
        {
            // TODO: handle 
        }
        if (inputParams.SyncInterval < 10)
        {
            throw new ArgumentException("Sync interval must be no lesser than 10 seconds");
        }
    }
}