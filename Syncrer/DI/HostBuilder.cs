using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Serilog;
using Syncrer.Inputs;
using Syncrer.Sync;
using Syncrer.Sync.Model;

namespace Syncrer.DI;

public class HostBuilder
{
    public IServiceProvider Provider { get; }
    public IHost Host { get; }

    public HostBuilder(string[] args, InputParams inputParams)
    {
        var builder = CreateHostBuilder(args, inputParams);
        Host = builder.Build();
        var serviceScope = Host.Services.CreateScope();
        Provider = serviceScope.ServiceProvider;
    }

    private static HostApplicationBuilder CreateHostBuilder(string[] args, InputParams inputParams)
    {
        var config = GetConfiguration();
        ConfigureLogger(config);

        var hostBuilder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args);
        ConfigureServices(args, inputParams, hostBuilder, config);

        return hostBuilder;
    }

    private static void ConfigureServices(
        string[] args,
        InputParams inputParams,
        HostApplicationBuilder hostBuilder,
        IConfigurationRoot config)
    {
        hostBuilder.Services.AddSingleton(args);
        hostBuilder.Services.AddSingleton(inputParams);
        hostBuilder.Services.AddSingleton<StartupSyncer>();
        hostBuilder.Services.AddSingleton<KnownModelStore>();
        hostBuilder.Services.AddQuartz();
        hostBuilder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
        hostBuilder.Services.AddSerilog(Log.Logger);
        hostBuilder.Services.AddSingleton(config);
    }

    private static void ConfigureLogger(IConfigurationRoot config)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .CreateLogger();
    }

    private static IConfigurationRoot GetConfiguration()
    {
        var configBuilder = new ConfigurationBuilder();
        configBuilder
            .SetBasePath(AppContext.BaseDirectory)
            .AddYamlFile("syncrer.config.yaml", optional: false, reloadOnChange: true);
        var config = configBuilder.Build();
        return config;
    }
}