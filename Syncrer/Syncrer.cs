namespace Syncrer;

/**
 * TODO:
 * 1. Source is 100 files. Destination is empty, Do the copy over.
 * 2. Do the copy over and over on schedule.
 * 3. Do the smart copy on schedule.
 * 4. Destination is not empty. Start with the smart copy.
 * 5. Source is 5k non-empty files
 * 6. Source is 500k empty files
 * 7. Source is 1m non-empty files
 * 8. Add support for empty folders
 */

internal static class Syncrer
{
    private static int Main(string[] args)
    {
        InputParams inputParams;
        try
        {
            inputParams = InputParser.GetParams(args);
            InputParser.VerifyParams(inputParams);
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e.Message);
            return 1;
        }
        Console.WriteLine($"Input: {string.Join(", ", inputParams.SourceFolder, inputParams.TargetFolder, inputParams.SyncInterval)}");
        
        Console.WriteLine("Starting Syncrer. Press Ctrl-C to stop Syncrer.");

        var scheduler = new Scheduler(inputParams);
        scheduler.Start();
        
        return 0;
    }
}