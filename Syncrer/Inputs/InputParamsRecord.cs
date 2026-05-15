namespace Syncrer.Inputs;

public record InputParamsRecord(
    DirectoryInfo SourceFolder,
    DirectoryInfo TargetFolder,
    int SyncInterval,
    bool AssumeYes = false);