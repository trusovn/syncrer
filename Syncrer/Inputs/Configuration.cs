using Microsoft.Extensions.Configuration;

namespace Syncrer.Inputs;

public class Configuration
{
    public Configuration(string[] args, IConfigurationRoot? configurationRoot = null)
    {
        ConfigurationRoot = configurationRoot ?? new ConfigurationBuilder().Build();
        InputParamsRecord parsedInput = InputParamsConfiguration.ParseParams(args);
        InputParamsVerifier.VerifyParams(parsedInput);
        SourceFolder = parsedInput.SourceFolder;
        TargetFolder = parsedInput.TargetFolder;
        SyncIntervalSeconds = parsedInput.SyncInterval;
        AssumeYes = parsedInput.AssumeYes;
    }

    public IConfigurationRoot ConfigurationRoot { get; }
    public DirectoryInfo SourceFolder { get; }
    public DirectoryInfo TargetFolder { get; }
    public int SyncIntervalSeconds { get; }
    public bool AssumeYes { get; }
    public string[] FileIgnorePatterns =>
        ConfigurationRoot.GetSection("fileIgnorePatterns").Get<string[]>() ?? [];
}
