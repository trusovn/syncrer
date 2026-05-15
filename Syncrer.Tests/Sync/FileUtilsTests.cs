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
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
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

    [Fact]
    public void CopyFiles_ReplacesExistingTargetDirectoryWithFile()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
        temp.CreateFile("source/collision", "content");
        temp.CreateDirectory("target/collision");

        var exception = Record.Exception(
            () => FileUtils.CopyFiles(
                ["collision"],
                source,
                target,
                SyncActionType.New,
                NullLogger.Instance));

        Assert.Null(exception);
        Assert.False(Directory.Exists(Path.Combine(target.FullName, "collision")));
        Assert.Equal("content", File.ReadAllText(Path.Combine(target.FullName, "collision")));
    }

    [Fact]
    public void CopyFiles_ReplacesBlockingFileWhenCreatingNestedTargetDirectory()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
        temp.CreateFile("source/nested/file.txt", "content");
        temp.CreateFile("target/nested", "blocking-file");

        var exception = Record.Exception(
            () => FileUtils.CopyFiles(
                ["nested/file.txt"],
                source,
                target,
                SyncActionType.New,
                NullLogger.Instance));

        Assert.Null(exception);
        Assert.True(Directory.Exists(Path.Combine(target.FullName, "nested")));
        Assert.Equal("content", File.ReadAllText(Path.Combine(target.FullName, "nested", "file.txt")));
    }
}
