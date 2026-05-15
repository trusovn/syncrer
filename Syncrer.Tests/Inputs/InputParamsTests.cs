using Syncrer.Inputs;

namespace Syncrer.Tests.Inputs;

public sealed class InputParamsTests
{
    [Fact]
    public void Constructor_AllowsMissingTargetFolderWhenParentExists()
    {
        using var temp = new TempDirectory();
        var source = temp.CreateDirectory("source");
        var target = new DirectoryInfo(temp.GetPath("target"));

        var inputParams = new InputParams(
            [
                "--source-folder", source.FullName,
                "--target-folder", target.FullName,
                "--sync-interval", "10",
            ]);

        Assert.Equal(source.FullName, inputParams.Params.SourceFolder.FullName);
        Assert.Equal(target.FullName, inputParams.Params.TargetFolder.FullName);
        Assert.Equal(10, inputParams.Params.SyncInterval);
        Assert.False(target.Exists);
    }

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
}
