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
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationValues)
            .Build();

        return new SyncExecutor(
            InputParamsUtils.CreateInputParams(source, target),
            knownModelStore,
            NullLogger<SyncExecutor>.Instance,
            configuration);
    }
}