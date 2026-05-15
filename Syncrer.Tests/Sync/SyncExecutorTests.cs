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
        var knownModel = ModelUtils.CreateKnownModel(target);

        var executor = CreateExecutor(source, target, knownModel);

        await executor.Execute(null!);

        Assert.Equal("new", await File.ReadAllTextAsync(Path.Combine(target.FullName, "nested", "new.txt")));
        Assert.Contains(
            knownModel.Model,
            record => record.RelativePath == Path.Combine("nested", "new.txt"));
    }

    [Fact]
    public async Task Execute_DeletesTargetFilesMissingFromSource()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
        temp.CreateFile("target/removed.txt", "old");
        var knownModel = ModelUtils.CreateKnownModel(target);

        var executor = CreateExecutor(source, target, knownModel);

        await executor.Execute(null!);

        Assert.False(File.Exists(Path.Combine(target.FullName, "removed.txt")));
        Assert.Empty(knownModel.Model);
    }

    [Fact]
    public async Task Execute_OverwritesModifiedTargetFiles()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
        temp.CreateFile("target/same.txt", "old");
        var knownModel = ModelUtils.CreateKnownModel(target);
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
        var knownModel = ModelUtils.CreateKnownModel(target);

        var executor = CreateExecutor(source, target, knownModel, ".DS_Store");

        await executor.Execute(null!);

        Assert.False(File.Exists(Path.Combine(target.FullName, ".DS_Store")));
        Assert.Empty(knownModel.Model);
    }

    [Fact]
    public async Task Execute_DoesNotCopyFilesMatchingWildcardIgnorePattern()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
        temp.CreateFile("source/._metadata", "metadata");
        var knownModel = ModelUtils.CreateKnownModel(target);

        var executor = CreateExecutor(source, target, knownModel, "._*");

        await executor.Execute(null!);

        Assert.False(File.Exists(Path.Combine(target.FullName, "._metadata")));
        Assert.Empty(knownModel.Model);
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
        var knownModel = ModelUtils.CreateKnownModel(target);
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
        var knownModel = ModelUtils.CreateKnownModel(target);
        await File.WriteAllTextAsync(targetPath, "manual");

        var executor = CreateExecutor(source, target, knownModel);

        await executor.Execute(null!);

        Assert.Equal("source", await File.ReadAllTextAsync(targetPath));
    }

    [Fact]
    public async Task Execute_ReplacesBlockingTargetDirectoryWithSourceFile()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
        temp.CreateFile("source/collision", "source");
        temp.CreateDirectory("target/collision");
        var knownModel = ModelUtils.CreateKnownModel(target);

        var executor = CreateExecutor(source, target, knownModel);

        var exception = await Record.ExceptionAsync(() => executor.Execute(null!));

        Assert.Null(exception);
        Assert.False(Directory.Exists(Path.Combine(target.FullName, "collision")));
        Assert.Equal("source", await File.ReadAllTextAsync(Path.Combine(target.FullName, "collision")));
        Assert.Contains(knownModel.Model, record => record.RelativePath == "collision");
    }

    [Fact]
    public async Task Execute_KeepsKnownModelWhenSourceScanFails()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
        temp.CreateFile("target/existing.txt", "existing");
        var knownModel = ModelUtils.CreateKnownModel(target);
        var previousModel = knownModel.Model.OrderBy(record => record.RelativePath).ToArray();
        var executor = CreateExecutor(source, target, knownModel);
        Directory.Delete(source.FullName, recursive: true);

        var exception = await Record.ExceptionAsync(() => executor.Execute(null!));

        Assert.Null(exception);
        Assert.Equal(previousModel, knownModel.Model.OrderBy(record => record.RelativePath).ToArray());
    }

    [Fact]
    public async Task Execute_AutoHealsOnNextRunAfterSourceScanFailureIsResolved()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
        var knownModel = ModelUtils.CreateKnownModel(target);
        var executor = CreateExecutor(source, target, knownModel);
        Directory.Delete(source.FullName, recursive: true);

        var firstRunException = await Record.ExceptionAsync(() => executor.Execute(null!));
        Directory.CreateDirectory(source.FullName);
        await File.WriteAllTextAsync(Path.Combine(source.FullName, "after-return.txt"), "source");

        var secondRunException = await Record.ExceptionAsync(() => executor.Execute(null!));

        Assert.Null(firstRunException);
        Assert.Null(secondRunException);
        Assert.Equal("source", await File.ReadAllTextAsync(Path.Combine(target.FullName, "after-return.txt")));
        Assert.Contains(knownModel.Model, record => record.RelativePath == "after-return.txt");
    }

    [Fact]
    public async Task Execute_DoesNotCopyFilesUnderLiteralIgnoredDirectory()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
        temp.CreateFile("source/.Spotlight-V100/index.txt", "metadata");
        var knownModel = ModelUtils.CreateKnownModel(target);

        var executor = CreateExecutor(source, target, knownModel, ".Spotlight-V100");

        await executor.Execute(null!);

        Assert.False(File.Exists(Path.Combine(target.FullName, ".Spotlight-V100", "index.txt")));
        Assert.Empty(knownModel.Model);
    }

    [Fact]
    public async Task Execute_DoesNotCopyFilesUnderWildcardIgnoredDirectory()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("target");
        temp.CreateFile("source/.Trash-100/file.txt", "metadata");
        var knownModel = ModelUtils.CreateKnownModel(target);

        var executor = CreateExecutor(source, target, knownModel, ".Trash-*");

        await executor.Execute(null!);

        Assert.False(File.Exists(Path.Combine(target.FullName, ".Trash-100", "file.txt")));
        Assert.Empty(knownModel.Model);
    }

    private static void MatchLastWriteTime(string sourcePath, string targetPath)
    {
        File.SetLastWriteTimeUtc(targetPath, File.GetLastWriteTimeUtc(sourcePath));
    }
}
