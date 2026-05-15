using Syncrer.Inputs;

namespace Syncrer.Tests.Utils;

public static class InputParamsUtils
{
    public static InputParams CreateInputParams(DirectoryInfo source, DirectoryInfo target, bool assumeYes = false)
    {
        var args = new List<string>
        {
            "--source-folder", source.FullName,
            "--target-folder", target.FullName,
            "--sync-interval", "10",
        };

        if (assumeYes)
        {
            args.Add("--yes");
        }

        return new InputParams(
            [..args]);
    }
}
