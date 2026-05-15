using Microsoft.Extensions.Logging.Abstractions;
using Syncrer.Sync;
using Syncrer.Sync.Model;

namespace Syncrer.Tests.Sync.Model;

public sealed class ModelUtilsTests
{
    [Fact]
    public void BuildModel_UsesRelativePathsAndCapturesMetadata()
    {
        using var temp = new TempDirectory();
        var filePath = temp.CreateFile("nested/file.txt", "content");
        var fileInfo = new FileInfo(filePath);

        FolderSnapshot folderSnapshot = ModelUtils.BuildModel(temp.DirectoryInfo, NullLogger.Instance);

        FileInfoRecord record = Assert.Single(folderSnapshot.Current);
        Assert.Equal(Path.Combine("nested", "file.txt"), record.RelativePath);
        Assert.Equal(fileInfo.Length, record.SizeBytes);
        Assert.Equal(fileInfo.LastWriteTimeUtc.Ticks, record.LastWriteTimeTicks);
    }

    [Fact]
    public void GetUniquePaths_CollapsesMultipleRecordsForTheSameRelativePath()
    {
        var model = new FolderSnapshot(
        [
            new FileInfoRecord("same.txt", 1, 10),
            new FileInfoRecord("same.txt", 2, 10),
            new FileInfoRecord("other.txt", 1, 10)
        ]);

        var uniquePaths = ModelUtils.GetUniquePaths(model);

        Assert.Equal(["other.txt", "same.txt"], uniquePaths.Order().ToArray());
    }
}
