using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Syncrer.Inputs;
using Syncrer.Sync;

namespace Syncrer.DI;

public class Builder
{
    public IServiceProvider provider { get; }
    public IHost host { get; }

    public Builder(string[] args)
    {
        var builder = CreateHostBuilder(args);
        host = builder.Build();
        var serviceScope = host.Services.CreateScope();
        provider = serviceScope.ServiceProvider;
    }

    private static HostApplicationBuilder CreateHostBuilder(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddSingleton(args);
        builder.Services.AddSingleton<InputParams>();
        builder.Services.AddSingleton<StartupSyncer>();
        builder.Services.AddSingleton<KnownModel>();
        builder.Services.AddQuartz();
        builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        return builder;
    }
}