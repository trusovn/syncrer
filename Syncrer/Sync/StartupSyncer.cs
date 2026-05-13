using Microsoft.Extensions.Logging;
using Syncrer.Inputs;
using Syncrer.Sync.Model;

namespace Syncrer.Sync;

public class StartupSyncer(InputParams inputParams, KnownModel model, ILogger<StartupSyncer> logger)
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