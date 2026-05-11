namespace Syncrer;

public record InputParams(DirectoryInfo SourceFolder, DirectoryInfo TargetFolder, int SyncInterval);