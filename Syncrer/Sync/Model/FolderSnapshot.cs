namespace Syncrer.Sync.Model;

public class FolderSnapshot
{
    private readonly HashSet<FileInfoRecord> _model;

    public IEnumerable<FileInfoRecord> Current => _model;

    public FolderSnapshot() : this(10000)
    {
    }

    public FolderSnapshot(int capacity)
    {
        _model = new HashSet<FileInfoRecord>(capacity);
    }

    public FolderSnapshot(IEnumerable<FileInfoRecord> source)
    {
        _model = new HashSet<FileInfoRecord>(source);
    }

    public int Count => _model.Count;

    public bool Add(FileInfoRecord file)
    {
        return _model.Add(file);
    }

    public IEnumerator<FileInfoRecord> GetEnumerator()
    {
        return _model.GetEnumerator();
    }

    public FolderSnapshot GetCopy()
    {
        return new FolderSnapshot(_model);
    }

    public void SymmetricExceptWith(FolderSnapshot other)
    {
        _model.SymmetricExceptWith(other._model);
    }
}