using Syncrer.Inputs;

namespace Syncrer.Tests.Inputs;

public sealed class InputParamsTests
{
    // [Fact]
    // public void Constructor_AllowsMissingTargetFolderWhenParentExists()
    // {
    //     using var temp = new TempDirectory();
    //     var source = temp.CreateDirectory("source");
    //     var target = new DirectoryInfo(temp.GetPath("target"));
    //
    //     var inputParams = new InputParams(
    //         [
    //             "--source-folder", source.FullName,
    //             "--target-folder", target.FullName,
    //             "--sync-interval", "10",
    //         ]);
    //
    //     Assert.Equal(source.FullName, inputParams.Params.SourceFolder.FullName);
    //     Assert.Equal(target.FullName, inputParams.Params.TargetFolder.FullName);
    //     Assert.Equal(10, inputParams.Params.SyncInterval);
    //     Assert.False(inputParams.Params.AssumeYes);
    //     Assert.False(target.Exists);
    // }

    [Fact]
    public void Constructor_HelpRequestThrowsWithSuccessExitCode()
    {
        var exception = Assert.Throws<InputParamsException>(() => new InputParams(["--help"]));

        Assert.Equal(0, exception.ExitCode);
        Assert.Equal(string.Empty, exception.Message);
    }

    [Fact]
    public void Constructor_ReportsAllValidationFailuresTogether()
    {
        using var temp = new TempDirectory();
        var missingSource = new DirectoryInfo(temp.GetPath("missing-source"));
        var targetWithMissingParent = new DirectoryInfo(temp.GetPath("missing-parent/target"));

        var exception = Assert.Throws<InputParamsException>(
            () => new InputParams(
                [
                    "--source-folder", missingSource.FullName,
                    "--target-folder", targetWithMissingParent.FullName,
                    "--sync-interval", "9",
                ]));

        Assert.Equal(1, exception.ExitCode);
        Assert.Contains("Source folder does not exist", exception.Message);
        Assert.Contains("Target folder parent does not exist", exception.Message);
        Assert.Contains("Sync interval must be no less than 10 seconds", exception.Message);
        Assert.Equal(3, exception.Message.Split(Environment.NewLine).Length);
    }

    [Fact]
    public void Constructor_ExceptionThrownForNestedSourceInTargetFolder()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("target/another/source");
        var target = temp.CreateDirectory("target");

        var exception = Assert.Throws<InputParamsException>(() => new InputParams(
        [
            "--source-folder", source.FullName,
            "--target-folder", target.FullName,
            "--sync-interval", "10"
        ]));

        Assert.Equal(1, exception.ExitCode);
        Assert.Contains("Target and Source folders can't be nested within each other", exception.Message);
    }

    [Fact]
    public void Constructor_ExceptionThrownForNestedTargetInSourceFolder()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("source/another/target");

        var exception = Assert.Throws<InputParamsException>(() => new InputParams(
        [
            "--source-folder", source.FullName,
            "--target-folder", target.FullName,
            "--sync-interval", "10"
        ]));

        Assert.Equal(1, exception.ExitCode);
        Assert.Contains("Target and Source folders can't be nested within each other", exception.Message);
    }

    [Fact]
    public void Constructor_ExceptionThrownForSameSourceAndTargetFolder()
    {
        using var temp = new TempDirectory();
        var folder = temp.CreateDirectory("shared");

        var exception = Assert.Throws<InputParamsException>(() => new InputParams(
        [
            "--source-folder", folder.FullName,
            "--target-folder", folder.FullName,
            "--sync-interval", "10",
        ]));

        Assert.Equal(1, exception.ExitCode);
        Assert.Contains("Target and Source folders can't be nested within each other", exception.Message);
    }

    [Fact]
    public void Constructor_AllowsSiblingFoldersWithCommonPathPrefix()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = temp.CreateDirectory("source-backup");

        var inputParams = new InputParams(
        [
            "--source-folder", source.FullName,
            "--target-folder", target.FullName,
            "--sync-interval", "10",
        ]);

        Assert.Equal(source.FullName, inputParams.Params.SourceFolder.FullName);
        Assert.Equal(target.FullName, inputParams.Params.TargetFolder.FullName);
    }

    // [Theory]
    // [InlineData("--yes")]
    // [InlineData("-Y")]
    // public void Constructor_ParsesAssumeYesOption(string option)
    // {
    //     using var temp = new TempDirectory();
    //     var source = temp.CreateDirectory("source");
    //     var target = temp.CreateDirectory("target");
    //
    //     var inputParams = new InputParams(
    //     [
    //         "--source-folder", source.FullName,
    //         "--target-folder", target.FullName,
    //         "--sync-interval", "10",
    //         option,
    //     ]);
    //
    //     Assert.True(inputParams.Params.AssumeYes);
    // }
}
