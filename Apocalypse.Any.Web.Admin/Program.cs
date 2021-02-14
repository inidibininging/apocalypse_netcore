//CHANGE THIS IN WINDOWS!
#define LINUX

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Apocalypse.Any.Web.Admin
{
    public static class Program
    {
#if LINUX
        /// <summary>
        ///   Initializes a new instance of the <see cref="WebHostBuilder"/> class with pre-configured defaults.
        /// </summary>
        /// <remarks>
        ///   The following defaults are applied to the returned <see cref="WebHostBuilder"/>:
        ///     use Kestrel as the web server and configure it using the application's configuration providers,
        ///     set the <see cref="Microsoft.AspNetCore.Hosting.IHostingEnvironment.ContentRootPath"/> to the result of <see cref="Directory.GetCurrentDirectory()"/>,
        ///     load <see cref="IConfiguration"/> from 'appsettings.json',
        ///     load <see cref="IConfiguration"/> from User Secrets when <see cref="Microsoft.AspNetCore.Hosting.IHostingEnvironment.EnvironmentName"/> is 'Development' using the entry assembly,
        ///     load <see cref="IConfiguration"/> from environment variables,
        ///     load <see cref="IConfiguration"/> from supplied command line args,
        ///     configures the <see cref="ILoggerFactory"/> to log to the console and debug output,
        ///     and enables the ability for frameworks to bind their options to their default configuration sections.
        /// </remarks>
        /// <param name="args">The command line args.</param>
        /// <returns>The initialized <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            System.Console.WriteLine("CreateWebHostBuilder");
            var builder = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

                    if (env.IsDevelopment())
                    {
                        var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                        if (appAssembly != null)
                        {
                            config.AddUserSecrets(appAssembly, optional: true);
                        }
                    }

                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                });


            if (args != null)
            {
                builder.UseConfiguration(new ConfigurationBuilder().AddCommandLine(args).Build());
            }

            return builder;
        }
#else
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
#endif

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
            .UseStartup<Startup>()
            .Build().Run();
        }
    }
}
