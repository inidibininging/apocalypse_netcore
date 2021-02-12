using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Sinks.SystemConsole.Themes;

namespace Apocalypse.Any.GameServer.Services
{
    public class LoggerServiceFactory
    {
        public LoggerServiceFactory()
        {
        }

        public ILogger<byte> GetLogger()
        {
            var providers = new LoggerProviderCollection();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.Providers(providers)
                .CreateLogger();

            var services = new ServiceCollection();

            // services.AddSingleton(providers);
            // services.AddSingleton<ILoggerFactory>(sc =>
            // {
            //     var providerCollection = sc.GetService<LoggerProviderCollection>();
            //     var factory = new SerilogLoggerFactory(null, true, providerCollection);
            //
            //     foreach (var provider in sc.GetServices<ILoggerProvider>())
            //         factory.AddProvider(provider);
            //
            //     return factory;
            // });

            services.AddLogging(l => l.AddSerilog());

            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<byte>>();

            return logger;
        }
    }
}