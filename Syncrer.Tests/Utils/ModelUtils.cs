using Microsoft.Extensions.Logging.Abstractions;
using Syncrer.Sync.Model;

namespace Syncrer.Tests.Utils;

public static class ModelUtils
{
    public static KnownModelStore CreateFolderModel(DirectoryInfo target)
    {
        var knownModel = new KnownModelStore(NullLogger<KnownModelStore>.Instance);
        knownModel.BuildNew(target);
        return knownModel;
    }
}