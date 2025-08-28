using SmartFactoryClient.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SmartFactoryClient.Services
{
    /// <summary>
    /// Main service that coordinates serial port communication and API data transmission
    /// </summary>
    public class SmartFactoryClientService : BackgroundService
    {
        private readonly ILogger<SmartFactoryClientService> _logger;
        private readonly SerialPortService _serialPortService;
        private readonly ApiClientService _apiClientService;
        private readonly DataParserService _dataParserService;
        private readonly ApplicationConfig _appConfig;
        
        private int _totalDataReceived = 0;
        private int _totalDataSent = 0;
        private int _totalErrors = 0;
        private DateTime _startTime;

        public SmartFactoryClientService(
            ILogger<SmartFactoryClientService> logger,
            SerialPortService serialPortService,
            ApiClientService apiClientService,
            DataParserService dataParserService,
            IOptions<ApplicationConfig> appConfig)
        {
            _logger = logger;
            _serialPortService = serialPortService;
            _apiClientService = apiClientService;
            _dataParserService = dataParserService;
            _appConfig = appConfig.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _startTime = DateTime.Now;
            
            _logger.LogInformation("🏭 Smart Factory Client Service starting...");

            try
            {
                // Test API connectivity first
                _logger.LogInformation("🔍 Testing API connectivity...");
                var isApiConnected = await _apiClientService.TestConnectivityAsync();
                
                if (!isApiConnected)
                {
                    _logger.LogWarning("⚠️ API connectivity test failed, but continuing anyway...");
                }

                // Initialize serial port
                _logger.LogInformation("📡 Initializing serial port communication...");
                var isSerialConnected = await _serialPortService.InitializeAsync();
                
                if (!isSerialConnected)
                {
                    _logger.LogError("❌ Failed to initialize serial port. Starting in simulation mode...");
                    await RunSimulationModeAsync(stoppingToken);
                    return;
                }

                // Subscribe to data received events
                _serialPortService.DataReceived += OnDataReceived;

                _logger.LogInformation("✅ Smart Factory Client Service started successfully");
                _logger.LogInformation("📊 Serial Port: {Status}", _serialPortService.GetConnectionStatus());
                _logger.LogInformation("🌐 API Status: {Status}", await _apiClientService.GetApiHealthAsync());

                // Display menu and handle user input
                await RunInteractiveMode(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Fatal error in Smart Factory Client Service");
            }
            finally
            {
                _serialPortService.DataReceived -= OnDataReceived;
                _serialPortService.Close();
                
                _logger.LogInformation("🛑 Smart Factory Client Service stopped");
                LogStatistics();
            }
        }

        /// <summary>
        /// Handle data received from serial port
        /// </summary>
        private async void OnDataReceived(object? sender, string rawData)
        {
            try
            {
                _totalDataReceived++;
                
                if (_appConfig.EnableConsoleOutput)
                {
                    _logger.LogInformation("📡 Data received from Proteus: {Data}", 
                        rawData.Replace("\n", " | "));
                }

                // Parse the sensor data
                var sensorData = _dataParserService.ParseSensorData(rawData);
                
                if (sensorData == null)
                {
                    _logger.LogWarning("⚠️ Failed to parse sensor data");
                    _totalErrors++;
                    return;
                }

                // Validate the data
                if (!_dataParserService.ValidateSensorData(sensorData))
                {
                    _logger.LogWarning("⚠️ Sensor data validation failed");
                    _totalErrors++;
                    return;
                }

                // Send to API
                var success = await _apiClientService.SendSensorDataAsync(rawData);
                
                if (success)
                {
                    _totalDataSent++;
                    if (_appConfig.EnableConsoleOutput)
                    {
                        _logger.LogInformation("✅ Data sent to API successfully: {SensorData}", sensorData);
                    }
                }
                else
                {
                    _totalErrors++;
                    _logger.LogError("❌ Failed to send data to API");
                }
            }
            catch (Exception ex)
            {
                _totalErrors++;
                _logger.LogError(ex, "❌ Error processing received data");
            }
        }

        /// <summary>
        /// Run in simulation mode when serial port is not available
        /// </summary>
        private async Task RunSimulationModeAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🎭 Running in simulation mode...");
            
            var random = new Random();
            var baseValues = new Dictionary<string, double>
            {
                ["Furnace_Temp"] = 850.0,
                ["Env_Humid"] = 50.0,
                ["Light_Level"] = 200,
                ["Gas_Methane"] = 20,
                ["Gas_CO"] = 50,
                ["Machine_Sound"] = 500,
                ["Tank_Pressure"] = 70,
                ["Main_Current"] = 120,
                ["Engine_Vibe"] = 20,
                ["Input_Voltage"] = 230,
                ["Conveyor_Dist"] = 150,
                ["Water_Leak"] = 100,
                ["Coolant_Valve"] = 85
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Generate simulated data with random variations
                    var simulatedData = GenerateSimulatedData(baseValues, random);
                    
                    _logger.LogInformation("🎭 Sending simulated data: {Data}", simulatedData.Replace("\n", " | "));
                    
                    var success = await _apiClientService.SendSensorDataAsync(simulatedData);
                    
                    if (success)
                    {
                        _totalDataSent++;
                        _logger.LogInformation("✅ Simulated data sent successfully");
                    }
                    else
                    {
                        _totalErrors++;
                        _logger.LogError("❌ Failed to send simulated data");
                    }

                    await Task.Delay(_appConfig.DataSendInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in simulation mode");
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }

        /// <summary>
        /// Generate simulated sensor data
        /// </summary>
        private string GenerateSimulatedData(Dictionary<string, double> baseValues, Random random)
        {
            var furnaceTemp = baseValues["Furnace_Temp"] + random.NextDouble() * 100 - 50;
            var humidity = Math.Max(0, Math.Min(100, baseValues["Env_Humid"] + random.NextDouble() * 20 - 10));
            var lightLevel = (int)Math.Max(0, baseValues["Light_Level"] + random.Next(-100, 100));
            var gasMethane = (int)Math.Max(0, baseValues["Gas_Methane"] + random.Next(-10, 30));
            var gasCO = (int)Math.Max(0, baseValues["Gas_CO"] + random.Next(-20, 40));
            var machineSound = (int)Math.Max(0, baseValues["Machine_Sound"] + random.Next(-200, 300));
            var tankPressure = (int)Math.Max(0, Math.Min(100, baseValues["Tank_Pressure"] + random.Next(-20, 30)));
            var mainCurrent = (int)Math.Max(0, baseValues["Main_Current"] + random.Next(-40, 80));
            var engineVibe = (int)Math.Max(0, baseValues["Engine_Vibe"] + random.Next(-10, 30));
            var inputVoltage = (int)Math.Max(0, baseValues["Input_Voltage"] + random.Next(-40, 40));
            var conveyorDist = (int)Math.Max(0, baseValues["Conveyor_Dist"] + random.Next(-100, 100));
            var waterLeak = (int)Math.Max(0, baseValues["Water_Leak"] + random.Next(-50, 200));
            var coolantValve = (int)Math.Max(0, Math.Min(100, baseValues["Coolant_Valve"] + random.Next(-20, 30)));

            // Random events (low probability)
            var flameStatus = random.NextDouble() < 0.02 ? 1 : 0;
            var gateStatus = random.NextDouble() < 0.1 ? 1 : 0;
            var eStopButton = random.NextDouble() < 0.005 ? 1 : 0;

            return $"Smart Factory Monitoring System - AUTOMATIC MODE - ONLINE\n" +
                   $"Furnace_Temp:{furnaceTemp:F2},Env_Humid:{humidity:F2},Light_Level:{lightLevel}," +
                   $"Gas_Methane:{gasMethane},Gas_CO:{gasCO},Machine_Sound:{machineSound}," +
                   $"Tank_Pressure:{tankPressure},Main_Current:{mainCurrent},Engine_Vibe:{engineVibe}," +
                   $"Input_Voltage:{inputVoltage},Conveyor_Dist:{conveyorDist},Water_Leak:{waterLeak}," +
                   $"Flame_Status:{flameStatus},Gate_Status:{gateStatus},E_Stop_Button:{eStopButton}," +
                   $"Coolant_Valve:{coolantValve}";
        }

        /// <summary>
        /// Run interactive mode with user menu
        /// </summary>
        private async Task RunInteractiveMode(CancellationToken stoppingToken)
        {
            DisplayMenu();
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true);
                        await HandleUserInput(key.KeyChar);
                    }

                    await Task.Delay(100, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Display user menu
        /// </summary>
        private void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("🏭 Smart Factory Client - Interactive Mode");
            Console.WriteLine(new string('=', 50));
            Console.WriteLine("Commands:");
            Console.WriteLine("  [S] Show Statistics");
            Console.WriteLine("  [T] Send Test Data");
            Console.WriteLine("  [C] Test API Connectivity");
            Console.WriteLine("  [P] Show Serial Port Status");
            Console.WriteLine("  [M] Show This Menu");
            Console.WriteLine("  [Q] Quit");
            Console.WriteLine(new string('=', 50));
            Console.WriteLine("Listening for data from Proteus...");
            Console.WriteLine("Press any key to show menu options.");
        }

        /// <summary>
        /// Handle user input
        /// </summary>
        private async Task HandleUserInput(char key)
        {
            switch (char.ToUpper(key))
            {
                case 'S':
                    LogStatistics();
                    break;
                case 'T':
                    await _apiClientService.SendTestDataAsync();
                    break;
                case 'C':
                    await _apiClientService.TestConnectivityAsync();
                    break;
                case 'P':
                    _logger.LogInformation("📡 Serial Port Status: {Status}", _serialPortService.GetConnectionStatus());
                    break;
                case 'M':
                    DisplayMenu();
                    break;
                case 'Q':
                    _logger.LogInformation("👋 Shutting down...");
                    Environment.Exit(0);
                    break;
            }
        }

        /// <summary>
        /// Log application statistics
        /// </summary>
        private void LogStatistics()
        {
            var uptime = DateTime.Now - _startTime;
            var successRate = _totalDataReceived > 0 ? (_totalDataSent * 100.0 / _totalDataReceived) : 0;
            
            Console.WriteLine();
            Console.WriteLine("📊 Application Statistics:");
            Console.WriteLine($"   ⏱️  Uptime: {uptime:hh\\:mm\\:ss}");
            Console.WriteLine($"   📡 Data Received: {_totalDataReceived}");
            Console.WriteLine($"   ✅ Data Sent: {_totalDataSent}");
            Console.WriteLine($"   ❌ Errors: {_totalErrors}");
            Console.WriteLine($"   📈 Success Rate: {successRate:F1}%");
            Console.WriteLine($"   🔗 Serial Port: {_serialPortService.GetConnectionStatus()}");
            Console.WriteLine();
        }
    }
}