using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Syncrer.DI;
using Syncrer.Inputs;

namespace Syncrer;

public static class Scheduler
{
    public static async Task ConfigureRunner(HostBuilder hostBuilder)
    {
        var inputParams = hostBuilder.Provider.GetRequiredService<InputParams>().Params;
        var schedulerFactory = hostBuilder.Provider.GetRequiredService<ISchedulerFactory>();

        var scheduler = await schedulerFactory.GetScheduler();
        var job = JobBuilder.Create<SyncExecutor>()
            .WithIdentity("scheduler", "group1")
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity("trigger1", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(inputParams.SyncInterval)
                .RepeatForever()
                .WithMisfireHandlingInstructionNextWithExistingCount())
            .Build();

        await scheduler.ScheduleJob(job, trigger);
    }
}