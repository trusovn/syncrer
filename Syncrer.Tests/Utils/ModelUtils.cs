using Microsoft.Extensions.Logging.Abstractions;
using Syncrer.Sync.Model;

namespace Syncrer.Tests.Utils;

public static class ModelUtils
{
    public static KnownModel CreateKnownModel(DirectoryInfo target)
    {
        var knownModel = new KnownModel(NullLogger<KnownModel>.Instance);
        knownModel.BuildNew(target);
        return knownModel;
    }
}