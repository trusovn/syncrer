namespace Syncrer.Inputs;

public static class InputParamsVerifier
{
    public static void VerifyParams(InputParamsRecord inputParamsRecord)
    {
        DirectoryInfo sourceFolder = inputParamsRecord.SourceFolder;
        DirectoryInfo targetFolder = inputParamsRecord.TargetFolder;

        VerifySource(sourceFolder);

        VerifyTarget(targetFolder);

        VerifySyncInterval(inputParamsRecord);

        VerifyNestedSourceTarget(sourceFolder, targetFolder);
    }

    private static void VerifyNestedSourceTarget(DirectoryInfo sourceFolder, DirectoryInfo targetFolder)
    {
        string sourcePath = NormalizeDirectoryPath(sourceFolder.FullName);
        string targetPath = NormalizeDirectoryPath(targetFolder.FullName);

        if (sourcePath == targetPath
            || IsSubdirectoryOf(sourcePath, targetPath)
            || IsSubdirectoryOf(targetPath, sourcePath))
        {
            throw InputParamsException.Invalid(
                ["Target and Source folders can't be nested within each other"]);
        }
    }

    private static void VerifySyncInterval(InputParamsRecord inputParamsRecord)
    {
        if (inputParamsRecord.SyncInterval < 10)
        {
            InputParamsException.Invalid(["Sync interval must be no less than 10 seconds"]);
        }
    }

    private static void VerifyTarget(DirectoryInfo targetFolder)
    {
        DirectoryInfo? targetParent = targetFolder.Parent;
        if (targetParent is null || !targetParent.Exists)
        {
            InputParamsException.Invalid([
                $"Target folder parent does not exist or cannot be accessed: {targetFolder.FullName}"
            ]);
        }
    }

    private static void VerifySource(DirectoryInfo sourceFolder)
    {
        if (!sourceFolder.Exists)
        {
            throw InputParamsException.Invalid([
                $"Source folder does not exist or cannot be accessed. Check source is a folder and has required permissions: {sourceFolder.FullName}"
            ]);
        }
    }

    private static string NormalizeDirectoryPath(string path)
    {
        return Path.GetFullPath(path)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    private static bool IsSubdirectoryOf(string childPath, string parentPath)
    {
        return childPath.StartsWith(parentPath + Path.DirectorySeparatorChar);
    }
}