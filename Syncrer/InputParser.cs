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
            return new InputParams()
            {
                SourceFolder = sourceFolder,
                TargetFolder = targetFolder,
                SyncInterval = syncInterval,
            };
        }
        throw new ArgumentException(string.Join(" ", parseResult.Errors));
    }
}

public struct InputParams
{
    public DirectoryInfo SourceFolder { get; set; }
    public DirectoryInfo TargetFolder { get; set; }
    public int SyncInterval { get; set; }
}