namespace Syncrer.Sync;

public class KnownModel
{
    public HashSet<FileInfoRecord> Model { get; private set; } = [];

    public void BuildNew(DirectoryInfo folder)
    {
        Model = ModelUtils.BuildModel(folder);
    }
    
    public void UpdateModel(HashSet<FileInfoRecord> model)
    {
        Model = model;
    }
}