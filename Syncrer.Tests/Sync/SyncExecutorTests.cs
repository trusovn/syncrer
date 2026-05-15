using Syncrer.Sync;
using Syncrer.Sync.Model;
using static Syncrer.Tests.Utils.SyncExecutorUtils;
using ModelUtils = Syncrer.Tests.Utils.ModelUtils;

namespace Syncrer.Tests.Sync;

public sealed class SyncExecutorTests
{
    [Fact]
    public async Task Execute_CopiesNewNestedFilesAndUpdatesKnownModel()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
        temp.CreateFile("source/nested/new.txt", "new");
        var knownModel = ModelUtils.CreateFolderModel(target);

        var executor = CreateExecutor(source, target, knownModel);

        await executor.Execute(null!);

        Assert.Equal("new", await File.ReadAllTextAsync(Path.Combine(target.FullName, "nested", "new.txt")));
        Assert.Contains(
            knownModel.FolderSnapshot.Current,
            record => record.RelativePath == Path.Combine("nested", "new.txt"));
    }

    [Fact]
    public async Task Execute_DeletesTargetFilesMissingFromSource()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
        temp.CreateFile("target/removed.txt", "old");
        var knownModel = ModelUtils.CreateFolderModel(target);

        var executor = CreateExecutor(source, target, knownModel);

        await executor.Execute(null!);

        Assert.False(File.Exists(Path.Combine(target.FullName, "removed.txt")));
        Assert.Empty(knownModel.FolderSnapshot.Current);
    }

    [Fact]
    public async Task Execute_OverwritesModifiedTargetFiles()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
        temp.CreateFile("target/same.txt", "old");
        var knownModel = ModelUtils.CreateFolderModel(target);
        temp.CreateFile("source/same.txt", "newer-content");

        var executor = CreateExecutor(source, target, knownModel);

        await executor.Execute(null!);

        Assert.Equal("newer-content", await File.ReadAllTextAsync(Path.Combine(target.FullName, "same.txt")));
    }

    [Fact]
    public async Task Execute_DoesNotCopyFilesMatchingLiteralIgnorePattern()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
        temp.CreateFile("source/.DS_Store", "metadata");
        var knownModel = ModelUtils.CreateFolderModel(target);

        var executor = CreateExecutor(source, target, knownModel, ".DS_Store");

        await executor.Execute(null!);

        Assert.False(File.Exists(Path.Combine(target.FullName, ".DS_Store")));
        Assert.Empty(knownModel.FolderSnapshot.Current);
    }

    [Fact]
    public async Task Execute_DoesNotCopyFilesMatchingWildcardIgnorePattern()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
        temp.CreateFile("source/._metadata", "metadata");
        var knownModel = ModelUtils.CreateFolderModel(target);

        var executor = CreateExecutor(source, target, knownModel, "._*");

        await executor.Execute(null!);

        Assert.False(File.Exists(Path.Combine(target.FullName, "._metadata")));
        Assert.Empty(knownModel.FolderSnapshot.Current);
    }

    [Fact]
    public async Task Execute_RecreatesTargetFileDeletedAfterKnownModelWasBuilt()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
        var sourcePath = temp.CreateFile("source/same.txt", "source");
        var targetPath = temp.CreateFile("target/same.txt", "source");
        MatchLastWriteTime(sourcePath, targetPath);
        var knownModel = ModelUtils.CreateFolderModel(target);
        File.Delete(targetPath);

        var executor = CreateExecutor(source, target, knownModel);

        await executor.Execute(null!);

        Assert.True(File.Exists(targetPath));
        Assert.Equal("source", await File.ReadAllTextAsync(targetPath));
    }

    [Fact]
    public async Task Execute_RevertsTargetFileModifiedAfterKnownModelWasBuilt()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
        var sourcePath = temp.CreateFile("source/same.txt", "source");
        var targetPath = temp.CreateFile("target/same.txt", "source");
        MatchLastWriteTime(sourcePath, targetPath);
        var knownModel = ModelUtils.CreateFolderModel(target);
        await File.WriteAllTextAsync(targetPath, "manual");

        var executor = CreateExecutor(source, target, knownModel);

        await executor.Execute(null!);

        Assert.Equal("source", await File.ReadAllTextAsync(targetPath));
    }

    [Fact]
    public async Task Execute_KeepsKnownModelWhenSourceScanFails()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
        temp.CreateFile("target/existing.txt", "existing");
        var folderModel = ModelUtils.CreateFolderModel(target);
        FileInfoRecord[] previousModel =
            folderModel.FolderSnapshot.Current.OrderBy(record => record.RelativePath).ToArray();
        var executor = CreateExecutor(source, target, folderModel);
        Directory.Delete(source.FullName, recursive: true);

        var exception = await Record.ExceptionAsync(() => executor.Execute(null!));

        Assert.Null(exception);
        Assert.Equal(previousModel, folderModel.FolderSnapshot.Current.OrderBy(record => record.RelativePath).ToArray());
    }

    [Fact]
    public async Task Execute_AutoHealsOnNextRunAfterSourceScanFailureIsResolved()
    {
        using var temp = new TempDirectory();
        DirectoryInfo source = temp.CreateDirectory("source");
        DirectoryInfo target = temp.CreateDirectory("target");
        KnownModelStore knownModelStore = ModelUtils.CreateFolderModel(target);
        SyncExecutor syncExecutor = CreateExecutor(source, target, knownModelStore);
        Directory.Delete(source.FullName, recursive: true);

        Exception? firstRunException = await Record.ExceptionAsync(() => syncExecutor.Execute(null!));
        Directory.CreateDirectory(source.FullName);
        await File.WriteAllTextAsync(Path.Combine(source.FullName, "after-return.txt"), "source");

        Exception? secondRunException = await Record.ExceptionAsync(() => syncExecutor.Execute(null!));

        Assert.Null(firstRunException);
        Assert.Null(secondRunException);
        Assert.Equal("source", await File.ReadAllTextAsync(Path.Combine(target.FullName, "after-return.txt")));
        Assert.Contains(knownModelStore.FolderSnapshot.Current, record => record.RelativePath == "after-return.txt");
    }

    [Fact]
    public async Task Execute_DoesNotCopyFilesUnderLiteralIgnoredDirectory()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
        temp.CreateFile("source/.Spotlight-V100/index.txt", "metadata");
        KnownModelStore knownModelStore = ModelUtils.CreateFolderModel(target);

        SyncExecutor executor = CreateExecutor(source, target, knownModelStore, ".Spotlight-V100");

        await executor.Execute(null!);

        Assert.False(File.Exists(Path.Combine(target.FullName, ".Spotlight-V100", "index.txt")));
        Assert.Empty(knownModelStore.FolderSnapshot.Current);
    }

    [Fact]
    public async Task Execute_DoesNotCopyFilesUnderWildcardIgnoredDirectory()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
        temp.CreateFile("source/.Trash-100/file.txt", "metadata");
        var knownModel = ModelUtils.CreateFolderModel(target);

        var executor = CreateExecutor(source, target, knownModel, ".Trash-*");

        await executor.Execute(null!);

        Assert.False(File.Exists(Path.Combine(target.FullName, ".Trash-100", "file.txt")));
        Assert.Empty(knownModel.FolderSnapshot.Current);
    }

    private static void MatchLastWriteTime(string sourcePath, string targetPath)
    {
        File.SetLastWriteTimeUtc(targetPath, File.GetLastWriteTimeUtc(sourcePath));
    }
}
