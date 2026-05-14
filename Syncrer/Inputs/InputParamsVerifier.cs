namespace Syncrer.Inputs;

public static class InputParamsVerifier
{
    public static void VerifyParams(InputParamsRecord inputParamsRecord)
    {
        var sourceFolder = inputParamsRecord.SourceFolder;
        var targetFolder = inputParamsRecord.TargetFolder;
        var errors = new List<string>();

        if (!sourceFolder.Exists)
        {
            errors.Add($"Source folder does not exist or cannot be accessed: {sourceFolder.FullName}");
        }

        var targetParent = targetFolder.Parent;
        if (targetParent is null || !targetParent.Exists)
        {
            errors.Add($"Target folder parent does not exist or cannot be accessed: {targetFolder.FullName}");
        }

        if (inputParamsRecord.SyncInterval < 10)
        {
            errors.Add("Sync interval must be no less than 10 seconds");
        }

        if (errors.Count > 0)
        {
            throw InputParamsException.Invalid(errors);
        }
    }
}