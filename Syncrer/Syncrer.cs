namespace Syncrer;

internal static class Syncrer
{
    private static int Main(string[] args)
    {
        InputParams inputParams;
        try
        {
            inputParams = InputParser.GetParams(args);
            if (!inputParams.SourceFolder.Exists)
            {
                throw new ArgumentException("Source folder does not exist");
            }
            if (inputParams.TargetFolder.Exists)
            {
                // TODO: handle 
            }
            if (inputParams.SyncInterval < 10)
            {
                throw new ArgumentException("Sync interval must be no lesser than 10 seconds");
            }
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e.Message);
            return 1;
        }
        Console.WriteLine($"Input: {string.Join(", ", inputParams.SourceFolder, inputParams.TargetFolder, inputParams.SyncInterval)}");
        
        Console.WriteLine("Starting Syncrer. Press Ctrl-C (Command-C) to stop Syncrer.");

        // TODO: do the rest :)
        
        return 0;
    }
}