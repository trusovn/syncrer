using Syncrer.Inputs;

namespace Syncrer.Tests.Utils;

public static class InputParamsUtils
{
    public static InputParams CreateInputParams(DirectoryInfo source, DirectoryInfo target)
    {
        return new InputParams(
        [
            "--source-folder", source.FullName,
            "--target-folder", target.FullName,
            "--sync-interval", "10",
        ]);
    }
}