using SmartFactoryClient.Models;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace SmartFactoryClient.Services
{
    /// <summary>
    /// Service for parsing sensor data received from Proteus
    /// </summary>
    public class DataParserService
    {
        private readonly ILogger<DataParserService> _logger;

        public DataParserService(ILogger<DataParserService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Parse the sensor data string received from Proteus
        /// Expected format: "Smart Factory Monitoring System - AUTOMATIC MODE - ONLINE\nFurnace_Temp:892.00,Env_Humid:0.00,..."
        /// </summary>
        public SensorData? ParseSensorData(string rawData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rawData))
                {
                    _logger.LogWarning("Received empty or null data");
                    return null;
                }

                _logger.LogDebug("Parsing sensor data: {RawData}", rawData);

                var sensorData = new SensorData
                {
                    RawData = rawData,
                    Timestamp = DateTime.Now
                };

                // Parse header information
                var lines = rawData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length < 2)
                {
                    _logger.LogError("Invalid data format - expected at least 2 lines");
                    return null;
                }

                // Parse the header line
                var headerLine = lines[0];
                if (headerLine.Contains("AUTOMATIC MODE"))
                {
                    sensorData.Mode = "AUTOMATIC";
                }
                else if (headerLine.Contains("MANUAL MODE"))
                {
                    sensorData.Mode = "MANUAL";
                }

                if (headerLine.Contains("ONLINE"))
                {
                    sensorData.SystemStatus = "ONLINE";
                }
                else if (headerLine.Contains("OFFLINE"))
                {
                    sensorData.SystemStatus = "OFFLINE";
                }

                // Parse sensor values from the data line
                var dataLine = lines[1];
                var sensorPairs = dataLine.Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (var pair in sensorPairs)
                {
                    var keyValue = pair.Split(':', 2);
                    if (keyValue.Length != 2)
                    {
                        _logger.LogWarning("Invalid sensor pair format: {Pair}", pair);
                        continue;
                    }

                    var key = keyValue[0].Trim();
                    var value = keyValue[1].Trim();

                    if (!ParseSensorValue(sensorData, key, value))
                    {
                        _logger.LogWarning("Failed to parse sensor value: {Key} = {Value}", key, value);
                    }
                }

                _logger.LogInformation("Successfully parsed sensor data: {SensorData}", sensorData);
                return sensorData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing sensor data: {RawData}", rawData);
                return null;
            }
        }

        /// <summary>
        /// Parse individual sensor value and assign to the appropriate property
        /// </summary>
        private bool ParseSensorValue(SensorData sensorData, string key, string value)
        {
            try
            {
                switch (key)
                {
                    case "Furnace_Temp":
                        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double furnaceTemp))
                        {
                            sensorData.Furnace_Temp = furnaceTemp;
                            return true;
                        }
                        break;

                    case "Env_Humid":
                        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double envHumid))
                        {
                            sensorData.Env_Humid = envHumid;
                            return true;
                        }
                        break;

                    case "Light_Level":
                        if (int.TryParse(value, out int lightLevel))
                        {
                            sensorData.Light_Level = lightLevel;
                            return true;
                        }
                        break;

                    case "Gas_Methane":
                        if (int.TryParse(value, out int gasMethane))
                        {
                            sensorData.Gas_Methane = gasMethane;
                            return true;
                        }
                        break;

                    case "Gas_CO":
                        if (int.TryParse(value, out int gasCO))
                        {
                            sensorData.Gas_CO = gasCO;
                            return true;
                        }
                        break;

                    case "Machine_Sound":
                        if (int.TryParse(value, out int machineSound))
                        {
                            sensorData.Machine_Sound = machineSound;
                            return true;
                        }
                        break;

                    case "Tank_Pressure":
                        if (int.TryParse(value, out int tankPressure))
                        {
                            sensorData.Tank_Pressure = tankPressure;
                            return true;
                        }
                        break;

                    case "Main_Current":
                        if (int.TryParse(value, out int mainCurrent))
                        {
                            sensorData.Main_Current = mainCurrent;
                            return true;
                        }
                        break;

                    case "Engine_Vibe":
                        if (int.TryParse(value, out int engineVibe))
                        {
                            sensorData.Engine_Vibe = engineVibe;
                            return true;
                        }
                        break;

                    case "Input_Voltage":
                        if (int.TryParse(value, out int inputVoltage))
                        {
                            sensorData.Input_Voltage = inputVoltage;
                            return true;
                        }
                        break;

                    case "Conveyor_Dist":
                        if (int.TryParse(value, out int conveyorDist))
                        {
                            sensorData.Conveyor_Dist = conveyorDist;
                            return true;
                        }
                        break;

                    case "Water_Leak":
                        if (int.TryParse(value, out int waterLeak))
                        {
                            sensorData.Water_Leak = waterLeak;
                            return true;
                        }
                        break;

                    case "Flame_Status":
                        if (int.TryParse(value, out int flameStatus))
                        {
                            sensorData.Flame_Status = flameStatus;
                            return true;
                        }
                        break;

                    case "Gate_Status":
                        if (int.TryParse(value, out int gateStatus))
                        {
                            sensorData.Gate_Status = gateStatus;
                            return true;
                        }
                        break;

                    case "E_Stop_Button":
                        if (int.TryParse(value, out int eStopButton))
                        {
                            sensorData.E_Stop_Button = eStopButton;
                            return true;
                        }
                        break;

                    case "Coolant_Valve":
                        if (int.TryParse(value, out int coolantValve))
                        {
                            sensorData.Coolant_Valve = coolantValve;
                            return true;
                        }
                        break;

                    default:
                        _logger.LogWarning("Unknown sensor key: {Key}", key);
                        return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing sensor value {Key} = {Value}", key, value);
            }

            return false;
        }

        /// <summary>
        /// Validate parsed sensor data
        /// </summary>
        public bool ValidateSensorData(SensorData sensorData)
        {
            if (sensorData == null)
                return false;

            // Basic validation rules
            var validationErrors = new List<string>();

            // Temperature validation
            if (sensorData.Furnace_Temp < 0 || sensorData.Furnace_Temp > 2000)
                validationErrors.Add($"Invalid furnace temperature: {sensorData.Furnace_Temp}");

            // Humidity validation
            if (sensorData.Env_Humid < 0 || sensorData.Env_Humid > 100)
                validationErrors.Add($"Invalid humidity: {sensorData.Env_Humid}");

            // Pressure validation
            if (sensorData.Tank_Pressure < 0 || sensorData.Tank_Pressure > 200)
                validationErrors.Add($"Invalid tank pressure: {sensorData.Tank_Pressure}");

            // Gas level validation
            if (sensorData.Gas_CO < 0 || sensorData.Gas_CO > 1000)
                validationErrors.Add($"Invalid CO level: {sensorData.Gas_CO}");

            if (sensorData.Gas_Methane < 0 || sensorData.Gas_Methane > 1000)
                validationErrors.Add($"Invalid methane level: {sensorData.Gas_Methane}");

            // Boolean sensors validation (should be 0 or 1)
            if (sensorData.Flame_Status < 0 || sensorData.Flame_Status > 1)
                validationErrors.Add($"Invalid flame status: {sensorData.Flame_Status}");

            if (sensorData.Gate_Status < 0 || sensorData.Gate_Status > 1)
                validationErrors.Add($"Invalid gate status: {sensorData.Gate_Status}");

            if (sensorData.E_Stop_Button < 0 || sensorData.E_Stop_Button > 1)
                validationErrors.Add($"Invalid emergency stop status: {sensorData.E_Stop_Button}");

            if (validationErrors.Any())
            {
                _logger.LogWarning("Sensor data validation failed: {Errors}", string.Join(", ", validationErrors));
                return false;
            }

            return true;
        }
    }
}