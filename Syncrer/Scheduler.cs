namespace Syncrer;

public class Scheduler(InputParams inputParams)
{
    public void Start()
    {
        var files = inputParams.SourceFolder.GetFiles("*.*", SearchOption.AllDirectories);

        CheckDestinationFolder(inputParams.TargetFolder);
        
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        foreach (var file in files)
        {
            var newFile = file.CopyTo(Path.Combine(inputParams.TargetFolder.FullName, file.Name), overwrite: true);
            Console.WriteLine($"Copied {file.Name} -> {newFile}");
        }
        sw.Stop();
        Console.WriteLine($"Took {sw.ElapsedMilliseconds} ms");
    }

    private void CheckDestinationFolder(DirectoryInfo targetFolder)
    {
        if (targetFolder.Exists)
        {
            Directory.Delete(targetFolder.FullName, true);
        }
        Directory.CreateDirectory(targetFolder.FullName);
    }
}