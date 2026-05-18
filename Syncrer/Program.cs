using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Syncrer.Inputs;
using Syncrer.Sync;
using HostBuilder = Syncrer.DI.HostBuilder;

namespace Syncrer;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        try
        {
            var inputParams = new InputParams(args);
            var builder = new HostBuilder(args, inputParams);

            builder.Provider.GetService<StartupSyncer>()!.Run();

            Console.WriteLine("Starting Syncrer. Press Ctrl-C to stop Syncrer.");
            Log.Logger.Information("Starting Syncrer.");

            await builder.Host.RunAsync();

            return 0;
        }
        catch (InputParamsException exception)
        {
            if (!string.IsNullOrWhiteSpace(exception.Message))
            {
                await Console.Error.WriteLineAsync(exception.Message);
                await Console.Error.WriteLineAsync();
            }

            await Console.Error.WriteAsync(InputParamsUsage.Text);
            return exception.ExitCode;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}