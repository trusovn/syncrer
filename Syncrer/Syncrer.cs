using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Logging;
using Microsoft.Extensions.Hosting;
using Syncrer.DI;
using Syncrer.Inputs;
using Syncrer.Logging;
using Syncrer.Sync;

namespace Syncrer;

/**
 * TODO:
 * 1. v Source is 100 files. Destination is empty, Do the copy over.
 * 2. v Do the copy over and over on schedule.
 * 3. Do the smart copy on schedule.
 * 3.5 Proper logging.
 * 4. Destination is not empty. Smart start.
 * 5. Source is 5k non-empty files
 * 6. Source is 500k empty files
 * 7. Source is 1m non-empty files
 * 8. Add support for empty folders
 */
internal class Syncrer
{
    private static async Task<int> Main(string[] args)
    {
        Console.WriteLine("Starting Syncrer. Press Ctrl-C to stop Syncrer.");
        
        var builder = new Builder(args);
        await RegisterRunner(builder);
        
        builder.provider.GetService<StartupSyncer>()!.Run();
        
        await builder.host.RunAsync();

        return 0;
    }

    private static async Task RegisterRunner(Builder builder)
    {
        LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());

        var inputParams = builder.provider.GetRequiredService<InputParams>().Params;
        var schedulerFactory = builder.provider.GetRequiredService<ISchedulerFactory>();

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