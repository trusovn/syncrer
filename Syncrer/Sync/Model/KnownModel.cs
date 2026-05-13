using Microsoft.Extensions.Logging;

namespace Syncrer.Sync.Model;

public class KnownModel(ILogger<KnownModel> logger)
{
    public HashSet<FileInfoRecord> Model { get; private set; } = [];

    public void BuildNew(DirectoryInfo folder)
    {
        Model = ModelUtils.BuildModel(folder, logger);
    }

    public void UpdateModel(HashSet<FileInfoRecord> model)
    {
        Model = model;
        logger.LogDebug("Model updated");
    }
}