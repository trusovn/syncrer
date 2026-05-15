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
}
