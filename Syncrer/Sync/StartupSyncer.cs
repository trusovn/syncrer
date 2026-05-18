using Syncrer.Inputs;
using Syncrer.Sync.Model;

namespace Syncrer.Sync;

public class StartupSyncer(Configuration configuration, KnownModelStore modelStore)
{
    public void Run()
    {
        if (!configuration.TargetFolder.Exists)
        {
            configuration.TargetFolder.Create();
        }

        modelStore.BuildNew(configuration.TargetFolder);
    }
}
