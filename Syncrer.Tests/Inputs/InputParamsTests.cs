using Syncrer.Inputs;

namespace Syncrer.Tests.Inputs;

public sealed class InputParamsTests
{
    [Fact]
    public void Constructor_AllowsMissingTargetFolderWhenParentExists()
    {
        using var temp = new TempDirectory();
        DirectoryInfo source = temp.CreateDirectory("source");
        DirectoryInfo target = new(temp.GetPath("target"));

        var inputParams = new InputParams(
        [
            "--source-folder", source.FullName,
            "--target-folder", target.FullName,
            "--sync-interval", "10"
        ]);

        Assert.Equal(source.FullName, inputParams.Params.SourceFolder.FullName);
        Assert.Equal(target.FullName, inputParams.Params.TargetFolder.FullName);
        Assert.Equal(10, inputParams.Params.SyncInterval);
        Assert.False(inputParams.Params.AssumeYes);
        Assert.False(target.Exists);
    }

    [Fact]
    public void Constructor_DoesntAllowSourceAsFile()
    {
        using var temp = new TempDirectory();
        string source = temp.CreateFile("source.txt", "source");
        DirectoryInfo target = new(temp.GetPath("target"));

        var exception = Assert.Throws<InputParamsException>(() => new InputParams(
        [
            "--source-folder", source,
            "--target-folder", target.FullName,
            "--sync-interval", "10"
        ]));

        Assert.Equal(1, exception.ExitCode);
        Assert.Contains(
            $"Source folder does not exist or cannot be accessed. Check source is a folder and has required permissions: {source}",
            exception.Message);
        Assert.Single(exception.Message.Split(Environment.NewLine));
    }

    [Fact]
    public void Constructor_HelpRequestThrowsWithSuccessExitCode()
    {
        var exception = Assert.Throws<InputParamsException>(() => new InputParams(["--help"]));

        Assert.Equal(0, exception.ExitCode);
        Assert.Equal(string.Empty, exception.Message);
    }

    [Fact]
    public void Constructor_ExceptionThrownForNestedSourceInTargetFolder()
    {
        using var temp = new TempDirectory();
        DirectoryInfo source = temp.CreateDirectory("target/another/source");
        DirectoryInfo target = temp.CreateDirectory("target");

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
        DirectoryInfo source = temp.CreateDirectory("source");
        DirectoryInfo target = temp.CreateDirectory("source/another/target");

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
        DirectoryInfo folder = temp.CreateDirectory("shared");

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
        DirectoryInfo source = temp.CreateDirectory("source");
        DirectoryInfo target = temp.CreateDirectory("source-backup");

        var inputParams = new InputParams(
        [
            "--source-folder", source.FullName,
            "--target-folder", target.FullName,
            "--sync-interval", "10",
        ]);

        Assert.Equal(source.FullName, inputParams.Params.SourceFolder.FullName);
        Assert.Equal(target.FullName, inputParams.Params.TargetFolder.FullName);
    }

    [Theory]
    [InlineData("--yes")]
    [InlineData("-Y")]
    public void Constructor_ParsesAssumeYesOption(string option)
    {
        using var temp = new TempDirectory();
        DirectoryInfo source = temp.CreateDirectory("source");
        DirectoryInfo target = temp.CreateDirectory("target");

        var inputParams = new InputParams(
        [
            "--source-folder", source.FullName,
            "--target-folder", target.FullName,
            "--sync-interval", "10",
            option
        ]);

        Assert.True(inputParams.Params.AssumeYes);
    }
}
