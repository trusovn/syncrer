using Syncrer.Inputs;
using Syncrer.Sync.Model;

namespace Syncrer.Sync;

public class StartupSyncer(InputParams inputParams, KnownModel model)
{
    public void Run()
    {
        if (!inputParams.Params.TargetFolder.Exists)
        {
            inputParams.Params.TargetFolder.Create();
        }

        model.BuildNew(inputParams.Params.TargetFolder);
    }
}