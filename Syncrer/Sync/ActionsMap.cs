using Syncrer.Inputs;

namespace Syncrer.Sync;

public class ActionsMap
{
    private readonly Configuration _configuration;
    private readonly Dictionary<SyncActionType, HashSet<string>> _actionsMap;

    public ActionsMap(HashSet<string> filesDiff, Configuration configuration)
    {
        _configuration = configuration;
        _actionsMap = BuildActionsMap(filesDiff);
    }

    public HashSet<string> GetDeleted()
    {
        return _actionsMap[SyncActionType.Deleted];
    }

    public HashSet<string> GetModified()
    {
        return _actionsMap[SyncActionType.Modified];
    }

    public HashSet<string> GetNew()
    {
        return _actionsMap[SyncActionType.New];
    }

    private Dictionary<SyncActionType, HashSet<string>> BuildActionsMap(HashSet<string> filesDiff)
    {
        var actionsMap = PrimaryContainerInit(filesDiff);

        // for every hit 'deleted is not present in target' - move to 'new' 
        UpdateWithEntriesForNew(actionsMap);

        // for every hit 'deleted is present in source' - move this to 'modified'
        UpdateWithEntriesForModified(actionsMap);

        return actionsMap;
    }

    private void UpdateWithEntriesForModified(Dictionary<SyncActionType, HashSet<string>> actionsMap)
    {
        foreach (var filePath in actionsMap[SyncActionType.Deleted])
        {
            string targetPath = Path.Combine(_configuration.SourceFolder.FullName, filePath);
            if (File.Exists(targetPath))
            {
                actionsMap[SyncActionType.Modified].Add(filePath);
            }
        }

        actionsMap[SyncActionType.Deleted].ExceptWith(actionsMap[SyncActionType.Modified]);
    }

    private void UpdateWithEntriesForNew(Dictionary<SyncActionType, HashSet<string>> actionsMap)
    {
        foreach (var filePath in actionsMap[SyncActionType.Deleted])
        {
            string targetPath = Path.Combine(_configuration.TargetFolder.FullName, filePath);
            if (!File.Exists(targetPath))
            {
                actionsMap[SyncActionType.New].Add(filePath);
            }
        }

        actionsMap[SyncActionType.Deleted].ExceptWith(actionsMap[SyncActionType.New]);
    }

    private static Dictionary<SyncActionType, HashSet<string>> PrimaryContainerInit(HashSet<string> filesDiff)
    {
        Dictionary<SyncActionType, HashSet<string>> actionsMap = new()
        {
            [SyncActionType.Deleted] = [..filesDiff],
            [SyncActionType.Modified] = [],
            [SyncActionType.New] = [],
        };
        return actionsMap;
    }
}
