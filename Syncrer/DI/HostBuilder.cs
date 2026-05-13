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

    public HostBuilder(string[] args)
    {
        var builder = CreateHostBuilder(args);
        Host = builder.Build();
        var serviceScope = Host.Services.CreateScope();
        Provider = serviceScope.ServiceProvider;
    }

    private static HostApplicationBuilder CreateHostBuilder(string[] args)
    {
        var config = GetConfiguration();
        ConfigureLogger(config);

        var hostBuilder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args);
        ConfigureServices(args, hostBuilder);

        return hostBuilder;
    }

    private static void ConfigureServices(string[] args, HostApplicationBuilder hostBuilder)
    {
        hostBuilder.Services.AddSingleton(args);
        hostBuilder.Services.AddSingleton<InputParams>();
        hostBuilder.Services.AddSingleton<StartupSyncer>();
        hostBuilder.Services.AddSingleton<KnownModel>();
        hostBuilder.Services.AddQuartz();
        hostBuilder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
        hostBuilder.Services.AddSerilog(Log.Logger);
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
        configBuilder.SetBasePath(Directory.GetCurrentDirectory())
            .AddYamlFile("syncrer.config.yaml", optional: false, reloadOnChange: true);
        var config = configBuilder.Build();
        return config;
    }
}