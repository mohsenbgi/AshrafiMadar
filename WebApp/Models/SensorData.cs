using System.ComponentModel.DataAnnotations;

namespace AshrafiMadar.Models
{
    public class SensorData
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Status { get; set; } = "ONLINE";
        public string Mode { get; set; } = "AUTOMATIC";
        
        // Temperature and Environmental
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
        
        // Warning Systems
        public bool Warning_LED { get; set; }
        public bool Alarm_System { get; set; }
        public List<string> ActiveWarnings { get; set; } = new List<string>();
        public List<string> ActiveAlarms { get; set; } = new List<string>();
    }

    public class SensorLocation
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public double? MinSafe { get; set; }
        public double? MaxSafe { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}