using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartFactoryClient.Models;
using SmartFactoryClient.Services;

namespace SmartFactoryClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Display startup banner
            DisplayBanner();

            try
            {
                // Build configuration
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddCommandLine(args)
                    .Build();

                // Create host
                var host = CreateHostBuilder(args, configuration).Build();

                // Run the application
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fatal error: {ex.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }

        static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
            Host.CreateDefaultBuilder(args)
                .UseConsoleLifetime()
                .ConfigureServices((hostContext, services) =>
                {
                    // Configure options
                    services.Configure<SmartFactoryConfig>(
                        configuration.GetSection("SmartFactory"));
                    services.Configure<SerialPortConfig>(
                        configuration.GetSection("SerialPort"));
                    services.Configure<ApplicationConfig>(
                        configuration.GetSection("Application"));

                    // Register services
                    services.AddSingleton<SerialPortService>();
                    services.AddSingleton<ApiClientService>();
                    services.AddSingleton<DataParserService>();
                    
                    // Register the main service as hosted service
                    services.AddHostedService<SmartFactoryClientService>();
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddSimpleConsole(options =>
                    {
                        options.IncludeScopes = false;
                        options.SingleLine = true;
                        options.TimestampFormat = "[HH:mm:ss] ";
                    });
                    
                    var logLevel = hostContext.Configuration.GetSection("Application:LogLevel").Value;
                    if (Enum.TryParse<LogLevel>(logLevel, out var level))
                    {
                        logging.SetMinimumLevel(level);
                    }
                });

        static void DisplayBanner()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
╔═══════════════════════════════════════════════════════════════╗
║                                                               ║
║     🏭 Smart Factory Client Application                      ║
║                                                               ║
║     📡 Proteus to API Data Bridge                           ║
║     🔗 Serial Port Communication                             ║
║     🌐 HTTP API Integration                                  ║
║                                                               ║
║     Version: 1.0.0                                           ║
║     Author: Smart Factory Team                               ║
║                                                               ║
╚═══════════════════════════════════════════════════════════════╝
");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("🚀 Starting Smart Factory Client...");
            Console.WriteLine();
        }
    }
}
