using Syncrer.Sync;
using static Syncrer.Tests.Utils.InputParamsUtils;

namespace Syncrer.Tests.Sync;

public sealed class ActionsMapTests
{
    [Fact]
    public void Constructor_ClassifiesMissingTargetFileAsNew()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
        temp.CreateFile("source/nested/new.txt", "new");

        var actionsMap = new ActionsMap(["nested/new.txt"], CreateInputParams(source, target));

        Assert.Equal(["nested/new.txt"], actionsMap.GetNew());
        Assert.Empty(actionsMap.GetModified());
        Assert.Empty(actionsMap.GetDeleted());
    }

    [Fact]
    public void Constructor_ClassifiesFilePresentInBothFoldersAsModified()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
        temp.CreateFile("source/same.txt", "new");
        temp.CreateFile("target/same.txt", "old");

        var actionsMap = new ActionsMap(["same.txt"], CreateInputParams(source, target));

        Assert.Equal(["same.txt"], actionsMap.GetModified());
        Assert.Empty(actionsMap.GetNew());
        Assert.Empty(actionsMap.GetDeleted());
    }

    [Fact]
    public void Constructor_ClassifiesMissingSourceFileAsDeleted()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
        temp.CreateFile("target/removed.txt", "old");

        var actionsMap = new ActionsMap(["removed.txt"], CreateInputParams(source, target));

        Assert.Equal(["removed.txt"], actionsMap.GetDeleted());
        Assert.Empty(actionsMap.GetNew());
        Assert.Empty(actionsMap.GetModified());
    }
}
