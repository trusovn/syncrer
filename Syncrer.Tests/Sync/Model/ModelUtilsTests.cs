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

        var model = ModelUtils.BuildModel(temp.DirectoryInfo, NullLogger.Instance);

        var record = Assert.Single(model);
        Assert.Equal(Path.Combine("nested", "file.txt"), record.RelativePath);
        Assert.Equal(fileInfo.Length, record.SizeBytes);
        Assert.Equal(fileInfo.LastWriteTimeUtc.Ticks, record.LastWriteTimeTicks);
    }

    [Fact]
    public void GetUniquePaths_CollapsesMultipleRecordsForTheSameRelativePath()
    {
        HashSet<FileInfoRecord> model =
        [
            new("same.txt", LastWriteTimeTicks: 1, SizeBytes: 10),
            new("same.txt", LastWriteTimeTicks: 2, SizeBytes: 10),
            new("other.txt", LastWriteTimeTicks: 1, SizeBytes: 10),
        ];

        var uniquePaths = ModelUtils.GetUniquePaths(model);

        Assert.Equal(["other.txt", "same.txt"], uniquePaths.Order().ToArray());
    }
}
