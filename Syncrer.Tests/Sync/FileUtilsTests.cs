using Microsoft.Extensions.Logging.Abstractions;
using Syncrer.Sync;

namespace Syncrer.Tests.Sync;

public sealed class FileUtilsTests
{
    [Fact]
    public void CopyFiles_CreatesNestedTargetDirectories()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
        temp.CreateFile("source/nested/file.txt", "content");

        FileUtils.CopyFiles(
            ["nested/file.txt"],
            source,
            target,
            SyncActionType.New,
            NullLogger.Instance);

        Assert.Equal("content", File.ReadAllText(Path.Combine(target.FullName, "nested", "file.txt")));
    }

    [Fact]
    public void CopyFiles_ContinuesWhenSourceFileDisappears()
    {
        using var temp = new TempDirectory();
        DirectoryInfo source = temp.CreateDirectory("source");
        DirectoryInfo target = temp.CreateDirectory("target");
        temp.CreateFile("source/available.txt", "available");

        var exception = Record.Exception(
            () => FileUtils.CopyFiles(
                ["missing.txt", "available.txt"],
                source,
                target,
                SyncActionType.New,
                NullLogger.Instance));

        Assert.Null(exception);
        Assert.Equal("available", File.ReadAllText(Path.Combine(target.FullName, "available.txt")));
        Assert.False(File.Exists(Path.Combine(target.FullName, "missing.txt")));
    }

}
