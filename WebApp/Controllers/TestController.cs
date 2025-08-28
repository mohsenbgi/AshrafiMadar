using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using AshrafiMadar.Models;
using AshrafiMadar.Services;
using AshrafiMadar.Hubs;

namespace AshrafiMadar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly SensorDataService _sensorDataService;
        private readonly IHubContext<SensorHub> _hubContext;
        private readonly ILogger<TestController> _logger;

        public TestController(SensorDataService sensorDataService, IHubContext<SensorHub> hubContext, ILogger<TestController> logger)
        {
            _sensorDataService = sensorDataService;
            _hubContext = hubContext;
            _logger = logger;
        }

        [HttpPost("simulate")]
        public async Task<ActionResult<ApiResponse<SensorData>>> SimulateSensorData()
        {
            try
            {
                // Sample sensor data string (like the one you provided)
                var sampleData = "Smart Factory Monitoring System - AUTOMATIC MODE - ONLINE\n" +
                    "Furnace_Temp:892.00,Env_Humid:45.50,Light_Level:210,Gas_Methane:15,Gas_CO:61," +
                    "Machine_Sound:512,Tank_Pressure:72,Main_Current:123,Engine_Vibe:25,Input_Voltage:230," +
                    "Conveyor_Dist:150,Water_Leak:100,Flame_Status:0,Gate_Status:0,E_Stop_Button:0,Coolant_Valve:90";

                var sensorData = _sensorDataService.ParseSensorString(sampleData);
                
                // Send real-time update to all connected clients
                await _hubContext.Clients.Group("SensorData").SendAsync("SensorDataUpdate", sensorData);
                
                return Ok(new ApiResponse<SensorData>
                {
                    Success = true,
                    Message = "داده‌های شبیه‌سازی شده با موفقیت ارسال شد",
                    Data = sensorData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در شبیه‌سازی داده‌های سنسور");
                return BadRequest(new ApiResponse<SensorData>
                {
                    Success = false,
                    Message = $"خطا در شبیه‌سازی: {ex.Message}"
                });
            }
        }

        [HttpPost("simulate-warning")]
        public async Task<ActionResult<ApiResponse<SensorData>>> SimulateWarningData()
        {
            try
            {
                // Sample data with warnings
                var warningData = "Smart Factory Monitoring System - AUTOMATIC MODE - ONLINE\n" +
                    "Furnace_Temp:950.00,Env_Humid:85.00,Light_Level:50,Gas_Methane:250,Gas_CO:85," +
                    "Machine_Sound:650,Tank_Pressure:85,Main_Current:160,Engine_Vibe:60,Input_Voltage:200," +
                    "Conveyor_Dist:0,Water_Leak:450,Flame_Status:0,Gate_Status:1,E_Stop_Button:0,Coolant_Valve:95";

                var sensorData = _sensorDataService.ParseSensorString(warningData);
                
                // Send real-time update to all connected clients
                await _hubContext.Clients.Group("SensorData").SendAsync("SensorDataUpdate", sensorData);
                
                return Ok(new ApiResponse<SensorData>
                {
                    Success = true,
                    Message = "داده‌های هشدار شبیه‌سازی شده با موفقیت ارسال شد",
                    Data = sensorData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در شبیه‌سازی داده‌های هشدار");
                return BadRequest(new ApiResponse<SensorData>
                {
                    Success = false,
                    Message = $"خطا در شبیه‌سازی: {ex.Message}"
                });
            }
        }

        [HttpPost("simulate-emergency")]
        public async Task<ActionResult<ApiResponse<SensorData>>> SimulateEmergencyData()
        {
            try
            {
                // Sample data with critical alarms
                var emergencyData = "Smart Factory Monitoring System - AUTOMATIC MODE - ONLINE\n" +
                    "Furnace_Temp:1050.00,Env_Humid:90.00,Light_Level:30,Gas_Methane:400,Gas_CO:120," +
                    "Machine_Sound:700,Tank_Pressure:95,Main_Current:180,Engine_Vibe:80,Input_Voltage:180," +
                    "Conveyor_Dist:0,Water_Leak:500,Flame_Status:1,Gate_Status:1,E_Stop_Button:1,Coolant_Valve:100";

                var sensorData = _sensorDataService.ParseSensorString(emergencyData);
                
                // Send real-time update to all connected clients
                await _hubContext.Clients.Group("SensorData").SendAsync("SensorDataUpdate", sensorData);
                
                return Ok(new ApiResponse<SensorData>
                {
                    Success = true,
                    Message = "داده‌های اضطراری شبیه‌سازی شده با موفقیت ارسال شد",
                    Data = sensorData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در شبیه‌سازی داده‌های اضطراری");
                return BadRequest(new ApiResponse<SensorData>
                {
                    Success = false,
                    Message = $"خطا در شبیه‌سازی: {ex.Message}"
                });
            }
        }

        [HttpGet("status")]
        public ActionResult<ApiResponse<object>> GetSystemStatus()
        {
            var status = new
            {
                SystemTime = DateTime.Now,
                Status = "ONLINE",
                Mode = "AUTOMATIC",
                ConnectedClients = "Active",
                LastUpdate = DateTime.Now
            };

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "وضعیت سیستم",
                Data = status
            });
        }
    }
}