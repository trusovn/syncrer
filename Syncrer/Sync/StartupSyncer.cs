using Syncrer.Inputs;
using Syncrer.Sync.Model;

namespace Syncrer.Sync;

public class StartupSyncer(InputParams inputParams, KnownModelStore modelStore)
{
    public void Run()
    {
        if (!inputParams.Params.TargetFolder.Exists)
        {
            inputParams.Params.TargetFolder.Create();
        }

        modelStore.BuildNew(inputParams.Params.TargetFolder);
    }
}