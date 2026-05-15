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
}
