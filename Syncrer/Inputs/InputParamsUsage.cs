namespace Syncrer.Inputs;

public static class InputParamsUsage
{
    public const string Text = """
                               Description:
                                 Syncrer app that synchronizes two folders (one way)

                               Usage:
                                 Syncrer --source-folder <source-folder> --target-folder <target-folder> --sync-interval <sync-interval>

                               Options:
                                 --source-folder <source-folder>      The path to the source folder.
                                 --target-folder <target-folder>      The path to the target folder.
                                 --sync-interval <sync-interval>      Synchronisation interval in seconds.
                                 -?, -h, --help                       Show help and usage information
                               """;
}