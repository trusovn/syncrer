using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Syncrer.Sync.Model;

namespace Syncrer.Tests.Utils;

public static class SyncExecutorUtils
{
    public static SyncExecutor CreateExecutor(
        DirectoryInfo source,
        DirectoryInfo target,
        KnownModelStore knownModelStore,
        params string[] ignorePatterns)
    {
        var configurationValues = ignorePatterns
            .Select((pattern, index) => new KeyValuePair<string, string?>($"fileIgnorePatterns:{index}", pattern));
        var configurationRoot = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationValues)
            .Build();
        var configuration = InputParamsUtils.CreateInputParams(
            source,
            target,
            configurationRoot: configurationRoot);

        return new SyncExecutor(
            configuration,
            knownModelStore,
            NullLogger<SyncExecutor>.Instance);
    }
}
