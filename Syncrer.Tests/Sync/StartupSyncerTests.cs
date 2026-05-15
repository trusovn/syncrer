using Microsoft.Extensions.Logging.Abstractions;
using Syncrer.Sync;
using Syncrer.Sync.Model;
using static Syncrer.Tests.Utils.InputParamsUtils;

namespace Syncrer.Tests.Sync;

public sealed class StartupSyncerTests
{
    // [Fact]
    // public void Run_ReturnsFalseWhenNonEmptyTargetIsRejected()
    // {
    //     using var temp = new TempDirectory();
    //     var source = temp.CreateDirectory("source");
    //     var target = temp.CreateDirectory("target");
    //     temp.CreateFile("target/existing.txt", "existing");
    //     var knownModel = new KnownModel(NullLogger<KnownModel>.Instance);
    //     var output = new StringWriter();
    //     var syncer = new StartupSyncer(
    //         CreateInputParams(source, target),
    //         knownModel,
    //         new StringReader("n"),
    //         output);
    //
    //     var result = syncer.Run();
    //
    //     Assert.False(result);
    //     Assert.Contains("Target folder is not empty", output.ToString());
    //     Assert.Empty(knownModel.Model);
    // }

    // [Fact]
    // public void Run_BuildsKnownModelWhenNonEmptyTargetIsConfirmed()
    // {
    //     using var temp = new TempDirectory();
    //     var source = temp.CreateDirectory("source");
    //     var target = temp.CreateDirectory("target");
    //     temp.CreateFile("target/existing.txt", "existing");
    //     var knownModel = new KnownModel(NullLogger<KnownModel>.Instance);
    //     var syncer = new StartupSyncer(
    //         CreateInputParams(source, target),
    //         knownModel,
    //         new StringReader("yes"),
    //         new StringWriter());
    //
    //     var result = syncer.Run();
    //
    //     Assert.True(result);
    //     Assert.Contains(knownModel.Model, record => record.RelativePath == "existing.txt");
    // }

    // [Fact]
    // public void Run_SkipsPromptWhenAssumeYesIsProvided()
    // {
    //     using var temp = new TempDirectory();
    //     var source = temp.CreateDirectory("source");
    //     var target = temp.CreateDirectory("target");
    //     temp.CreateFile("target/existing.txt", "existing");
    //     var knownModel = new KnownModel(NullLogger<KnownModel>.Instance);
    //     var output = new StringWriter();
    //     var syncer = new StartupSyncer(
    //         CreateInputParams(source, target, assumeYes: true),
    //         knownModel,
    //         new StringReader(""),
    //         output);
    //
    //     var result = syncer.Run();
    //
    //     Assert.True(result);
    //     Assert.Empty(output.ToString());
    //     Assert.Contains(knownModel.Model, record => record.RelativePath == "existing.txt");
    // }

    // [Fact]
    // public void Run_ReturnsFalseWhenTargetDirectoryConflictsWithSourceFile()
    // {
    //     using var temp = new TempDirectory();
    //     var source = temp.CreateDirectory("source");
    //     var target = temp.CreateDirectory("target");
    //     temp.CreateFile("source/collision", "source-file");
    //     temp.CreateDirectory("target/collision");
    //     var knownModel = new KnownModel(NullLogger<KnownModel>.Instance);
    //     var output = new StringWriter();
    //     var syncer = new StartupSyncer(
    //         CreateInputParams(source, target, assumeYes: true),
    //         knownModel,
    //         new StringReader(""),
    //         output);
    //
    //     var result = syncer.Run();
    //
    //     Assert.False(result);
    //     Assert.Contains("Target cannot be safely managed", output.ToString());
    //     Assert.Contains("collision", output.ToString());
    //     Assert.Contains("target is a directory but source is a file", output.ToString());
    //     Assert.True(Directory.Exists(Path.Combine(target.FullName, "collision")));
    //     Assert.Empty(knownModel.Model);
    // }

    // [Fact]
    // public void Run_ReturnsFalseWhenTargetFileBlocksSourceDirectory()
    // {
    //     using var temp = new TempDirectory();
    //     var source = temp.CreateDirectory("source");
    //     var target = temp.CreateDirectory("target");
    //     temp.CreateFile("source/nested/file.txt", "source-file");
    //     temp.CreateFile("target/nested", "blocking-target-file");
    //     var knownModel = new KnownModel(NullLogger<KnownModel>.Instance);
    //     var output = new StringWriter();
    //     var syncer = new StartupSyncer(
    //         CreateInputParams(source, target, assumeYes: true),
    //         knownModel,
    //         new StringReader(""),
    //         output);
    //
    //     var result = syncer.Run();
    //
    //     Assert.False(result);
    //     Assert.Contains("Target cannot be safely managed", output.ToString());
    //     Assert.Contains("nested", output.ToString());
    //     Assert.Contains("target is a file but source is a directory", output.ToString());
    //     Assert.Equal("blocking-target-file", File.ReadAllText(Path.Combine(target.FullName, "nested")));
    //     Assert.Empty(knownModel.Model);
    // }

    // [Fact]
    // public void Run_ReportsAllTargetShapeConflictsBeforeFailing()
    // {
    //     using var temp = new TempDirectory();
    //     var source = temp.CreateDirectory("source");
    //     var target = temp.CreateDirectory("target");
    //     temp.CreateFile("source/file-in-source", "source-file");
    //     temp.CreateDirectory("target/file-in-source");
    //     temp.CreateFile("source/directory-in-source/file.txt", "source-file");
    //     temp.CreateFile("target/directory-in-source", "blocking-target-file");
    //     var knownModel = new KnownModel(NullLogger<KnownModel>.Instance);
    //     var output = new StringWriter();
    //     var syncer = new StartupSyncer(
    //         CreateInputParams(source, target, assumeYes: true),
    //         knownModel,
    //         new StringReader(""),
    //         output);
    //
    //     var result = syncer.Run();
    //
    //     Assert.False(result);
    //     Assert.Contains("file-in-source", output.ToString());
    //     Assert.Contains("directory-in-source", output.ToString());
    //     Assert.Empty(knownModel.Model);
    // }
}
