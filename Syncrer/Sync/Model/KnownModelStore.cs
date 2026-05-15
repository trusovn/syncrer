using Microsoft.Extensions.Logging;

namespace Syncrer.Sync.Model;

public class KnownModelStore(ILogger<KnownModelStore> logger)
{
    private const int DefaultEstimatedFileCount = 10_000;

    public FolderSnapshot FolderSnapshot { get; private set; } = new();

    public void BuildNew(DirectoryInfo folder)
    {
        int estimatedFileCount = Math.Max(FolderSnapshot.Count, DefaultEstimatedFileCount);
        FolderSnapshot = ModelUtils.BuildModel(folder, logger, estimatedFileCount);
    }

    public void UpdateModel(FolderSnapshot snapshot)
    {
        FolderSnapshot = snapshot;
        logger.LogDebug("Model updated");
    }
}
