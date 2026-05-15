using Microsoft.Extensions.Logging;

namespace Syncrer.Sync.Model;

public class KnownModelStore(ILogger<KnownModelStore> logger)
{
    public FolderSnapshot FolderSnapshot { get; private set; }

    public void BuildNew(DirectoryInfo folder)
    {
        FolderSnapshot = ModelUtils.BuildModel(folder, logger);
    }

    public void UpdateModel(FolderSnapshot snapshot)
    {
        FolderSnapshot = snapshot;
        logger.LogDebug("Model updated");
    }
}