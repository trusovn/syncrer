using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Microsoft.Extensions.Hosting;
using Serilog;
using Syncrer.Inputs;
using Syncrer.Sync;
using HostBuilder = Syncrer.DI.HostBuilder;

namespace Syncrer;

/**
 * TODO:
 * 1. v Source is 100 files. Destination is empty, Do the copy over.
 * 2. v Do the copy over and over on schedule.
 * 3. v Do the smart copy on schedule.
 * 3.5. v Logging to console and file.
 * 3.6. v Log levels
 * 3.7. v Don't log 'deleted' for modified files. Maybe need to rethink the detection logic for this.
 * 4. v Destination is not empty. Smart start.
 * 4.5. v Configuration for file exclusions (e.g. .DS_Store) and log location.
 * 5. Source is 5k non-empty files
 * 6. Source is 500k empty files
 * 7. Source is 1m non-empty files
 * 7.5. Investigate-fix 'interval smaller than sync time'
 * 8. Add support for empty folders
 * 9. Gracefully handle copying and other errors
 * 10. Handle 'target modified'. Likely, need to store hash of the whole folder and restart if that changes.
 */
internal static class Syncrer
{
    private static async Task<int> Main(string[] args)
    {
        var builder = new HostBuilder(args);
        await ConfigureRunner(builder);

        builder.Provider.GetService<StartupSyncer>()!.Run();

        Console.WriteLine("Starting Syncrer. Press Ctrl-C to stop Syncrer.");
        Log.Logger.Information("Starting Syncrer.");
        await builder.Host.RunAsync();

        return 0;
    }

    private static async Task ConfigureRunner(HostBuilder hostBuilder)
    {
        var inputParams = hostBuilder.Provider.GetRequiredService<InputParams>().Params;
        var schedulerFactory = hostBuilder.Provider.GetRequiredService<ISchedulerFactory>();

        var scheduler = await schedulerFactory.GetScheduler();
        var job = JobBuilder.Create<Scheduler>()
            .WithIdentity("scheduler", "group1")
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity("trigger1", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(inputParams.SyncInterval)
                .RepeatForever())
            .Build();

        await scheduler.ScheduleJob(job, trigger);
    }
}