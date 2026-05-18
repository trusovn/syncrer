using Microsoft.Extensions.Configuration;
using Syncrer.Inputs;

namespace Syncrer.Tests.Utils;

public static class InputParamsUtils
{
    public static Configuration CreateInputParams(
        DirectoryInfo source,
        DirectoryInfo target,
        bool assumeYes = false,
        IConfigurationRoot? configurationRoot = null)
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

        configurationRoot ??= new ConfigurationBuilder().Build();
        return new Configuration([..args], configurationRoot);
    }
}
