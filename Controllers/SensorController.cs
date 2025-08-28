using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using AshrafiMadar.Models;
using AshrafiMadar.Services;
using AshrafiMadar.Hubs;

namespace AshrafiMadar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorController : ControllerBase
    {
        private readonly SensorDataService _sensorDataService;
        private readonly IHubContext<SensorHub> _hubContext;
        private readonly ILogger<SensorController> _logger;

        public SensorController(SensorDataService sensorDataService, IHubContext<SensorHub> hubContext, ILogger<SensorController> logger)
        {
            _sensorDataService = sensorDataService;
            _hubContext = hubContext;
            _logger = logger;
        }

        [HttpPost("data")]
        public async Task<ActionResult<ApiResponse<SensorData>>> ReceiveSensorData([FromBody] string sensorString)
        {
            try
            {
                _logger.LogInformation($"Received sensor data: {sensorString}");
                
                var sensorData = _sensorDataService.ParseSensorString(sensorString);
                
                // Send real-time update to all connected clients
                await _hubContext.Clients.Group("SensorData").SendAsync("SensorDataUpdate", sensorData);
                
                return Ok(new ApiResponse<SensorData>
                {
                    Success = true,
                    Message = "داده‌های سنسور با موفقیت دریافت شد",
                    Data = sensorData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت داده‌های سنسور");
                return BadRequest(new ApiResponse<SensorData>
                {
                    Success = false,
                    Message = $"خطا در پردازش داده‌ها: {ex.Message}"
                });
            }
        }

        [HttpPost("raw")]
        public async Task<ActionResult<ApiResponse<SensorData>>> ReceiveRawSensorData()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var sensorString = await reader.ReadToEndAsync();
                
                _logger.LogInformation($"Received raw sensor data: {sensorString}");
                
                var sensorData = _sensorDataService.ParseSensorString(sensorString);
                
                // Send real-time update to all connected clients
                await _hubContext.Clients.Group("SensorData").SendAsync("SensorDataUpdate", sensorData);
                
                return Ok(new ApiResponse<SensorData>
                {
                    Success = true,
                    Message = "داده‌های سنسور با موفقیت دریافت شد",
                    Data = sensorData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت داده‌های خام سنسور");
                return BadRequest(new ApiResponse<SensorData>
                {
                    Success = false,
                    Message = $"خطا در پردازش داده‌ها: {ex.Message}"
                });
            }
        }

        [HttpGet("current")]
        public ActionResult<ApiResponse<SensorData>> GetCurrentData()
        {
            try
            {
                var currentData = _sensorDataService.GetCurrentData();
                return Ok(new ApiResponse<SensorData>
                {
                    Success = true,
                    Message = "داده‌های فعلی سنسور",
                    Data = currentData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت داده‌های فعلی");
                return BadRequest(new ApiResponse<SensorData>
                {
                    Success = false,
                    Message = $"خطا در دریافت داده‌ها: {ex.Message}"
                });
            }
        }

        [HttpGet("locations")]
        public ActionResult<ApiResponse<List<SensorLocation>>> GetSensorLocations()
        {
            try
            {
                var locations = _sensorDataService.GetSensorLocations();
                return Ok(new ApiResponse<List<SensorLocation>>
                {
                    Success = true,
                    Message = "لیست موقعیت‌های سنسور",
                    Data = locations
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت موقعیت‌های سنسور");
                return BadRequest(new ApiResponse<List<SensorLocation>>
                {
                    Success = false,
                    Message = $"خطا در دریافت داده‌ها: {ex.Message}"
                });
            }
        }
    }
}