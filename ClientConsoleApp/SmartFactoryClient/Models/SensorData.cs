using System.ComponentModel.DataAnnotations;

namespace SmartFactoryClient.Models
{
    /// <summary>
    /// Represents sensor data received from Proteus
    /// </summary>
    public class SensorData
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string SystemStatus { get; set; } = "ONLINE";
        public string Mode { get; set; } = "AUTOMATIC";
        public string RawData { get; set; } = string.Empty;
        
        // Temperature and Environmental Sensors
        public double Furnace_Temp { get; set; }
        public double Env_Humid { get; set; }
        public int Light_Level { get; set; }
        
        // Gas Sensors
        public int Gas_Methane { get; set; }
        public int Gas_CO { get; set; }
        
        // Machine Monitoring
        public int Machine_Sound { get; set; }
        public int Tank_Pressure { get; set; }
        public int Main_Current { get; set; }
        public int Engine_Vibe { get; set; }
        public int Input_Voltage { get; set; }
        
        // Production Line
        public int Conveyor_Dist { get; set; }
        public int Water_Leak { get; set; }
        public int Flame_Status { get; set; }
        public int Gate_Status { get; set; }
        
        // Control Systems
        public int E_Stop_Button { get; set; }
        public int Coolant_Valve { get; set; }

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss}] Temp: {Furnace_Temp}°C, Pressure: {Tank_Pressure} PSI, " +
                   $"Gas_CO: {Gas_CO} ppm, Status: {SystemStatus}";
        }
    }

    /// <summary>
    /// Configuration for Smart Factory API
    /// </summary>
    public class SmartFactoryConfig
    {
        public string ApiBaseUrl { get; set; } = "http://localhost:5000/api";
        public string ApiEndpoint { get; set; } = "/sensor/raw";
        public int RetryAttempts { get; set; } = 3;
        public int RetryDelay { get; set; } = 2000;
    }

    /// <summary>
    /// Configuration for Serial Port
    /// </summary>
    public class SerialPortConfig
    {
        public string PortName { get; set; } = "COM1";
        public int BaudRate { get; set; } = 9600;
        public string Parity { get; set; } = "None";
        public int DataBits { get; set; } = 8;
        public string StopBits { get; set; } = "One";
        public string Handshake { get; set; } = "None";
        public int ReadTimeout { get; set; } = 5000;
        public int WriteTimeout { get; set; } = 5000;
    }

    /// <summary>
    /// Application configuration
    /// </summary>
    public class ApplicationConfig
    {
        public string LogLevel { get; set; } = "Information";
        public int DataSendInterval { get; set; } = 1000;
        public int MaxRetries { get; set; } = 5;
        public bool EnableConsoleOutput { get; set; } = true;
        public bool EnableFileLogging { get; set; } = false;
    }

    /// <summary>
    /// API response model
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}