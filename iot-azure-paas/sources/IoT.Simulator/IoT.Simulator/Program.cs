using IoT.Simulator.Exceptions;
using IoT.Simulator.Services;
using IoT.Simulator.Settings;
using IoT.Simulator.Tools;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace IoT.Simulator
{
    class Program
    {
        private static DeviceClient _deviceClient;

        // The device connection string to authenticate the device with your IoT hub.
        // Using the Azure CLI:
        // az iot hub device-identity show-connection-string --hub-name {YourIoTHubName} --device-id MyDotnetDevice --output table
        private static string _iotHubConnectionString;
        private static string _environmentName;

        public static IConfiguration Configuration { get; set; }

        private static void Main(string[] args)
        {
            Console.WriteLine("=======================================================================");
            Console.WriteLine("=                      IOT SIMULATOR - PROGRAMMEZ                     =");
            Console.WriteLine("=======================================================================");
            Console.WriteLine(AssemblyInformationHelper.HeaderMessage);
            Console.WriteLine("=======================================================================");
            Console.WriteLine(">> Loading configurations....");

            try
            {
                //Configuration
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile("devicesettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables();

                _environmentName = Environment.GetEnvironmentVariable("ENVIRONMENT");

                if (string.IsNullOrWhiteSpace(_environmentName))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("No environment platform has been found. Default setting: Development.");
                    _environmentName = "Development";
                    Console.ResetColor();
                }

                try
                {
                    ConfigurationHelpers.CheckEnvironmentConfigurationFiles(_environmentName);
                }
                catch (MissingEnvironmentConfigurationFileException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                    Console.WriteLine("Execution will continue with default settings in appsettings.json and devicesettings.json.");
                }

                builder.AddJsonFile($"appsettings.{_environmentName}.json", optional: true, reloadOnChange: true);
                builder.AddJsonFile($"devicesettings.{_environmentName}.json", optional: true, reloadOnChange: true);


                Configuration = builder.Build();

                IServiceCollection services = new ServiceCollection();

                ConfigureServices(services);

                var deviceSettings = Configuration.Get<DeviceSettings>();
                if (deviceSettings == null)
                    throw new ArgumentException("No device settings have been configured.");

                if (deviceSettings.SimulationSettings == null)
                    throw new ArgumentException("No device simulation settings have been configured.");

                if (deviceSettings.SimulationSettings.EnableDevice)
                {                 
                    RegisterMessagingServices(services);                 
                    RegisterDeviceSimulators(services);
                }

                IServiceProvider serviceProvider = services.BuildServiceProvider();

                //Logger
                var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();
                logger.LogDebug("PROGRAM::Settings, DI and logger configured and ready to use.");

                //Simulators
                if (!deviceSettings.SimulationSettings.EnableDevice)
                    logger.LogDebug("PROGRAM::No simulator has been configured.");
                else
                {
                    if (deviceSettings.SimulationSettings.EnableDevice)
                        StartDevicesSimulators(serviceProvider, logger);
                }

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
            finally
            {
                Console.ReadLine();
            }
        }


        static void ConfigureServices(IServiceCollection services)
        {
            if (services != null)
            {
                services.AddLogging(
                    loggingBuilder =>
                    {
                        loggingBuilder.ClearProviders();

                        //log level configuration
                        var loggingConfiguration = Configuration.GetSection("Logging");
                        loggingBuilder.AddConfiguration(loggingConfiguration);

                        if (_environmentName != "Production")
                        {
                            loggingBuilder.AddConsole();
                            loggingBuilder.AddDebug();
                        }
                    }
                    );

                services.AddOptions();

                services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
                services.Configure<DeviceSettings>(Configuration);

            }
        }

        static void RegisterDeviceSimulators(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton<ISimulationService, DeviceSimulationService>();
        }

        static void RegisterMessagingServices(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddTransient<ITelemetryMessageService, SimpleTelemetryMessageService>();
        }

        static void StartDevicesSimulators(IServiceProvider serviceProvider, ILogger logger)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            var simulators = serviceProvider.GetServices<ISimulationService>();
            if (simulators != null && simulators.Any())
            {
                foreach (var item in simulators)
                {
                    item.InitiateSimulationAsync();
                }

                logger.LogDebug($"DEVICES: {simulators.Count()} device simulator(s) initialized and running.");
            }
        }
    }
}
