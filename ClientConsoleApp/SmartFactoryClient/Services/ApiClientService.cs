using SmartFactoryClient.Models;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SmartFactoryClient.Services
{
    /// <summary>
    /// Service for sending data to Smart Factory API
    /// </summary>
    public class ApiClientService : IDisposable
    {
        private readonly ILogger<ApiClientService> _logger;
        private readonly SmartFactoryConfig _config;
        private readonly HttpClient _httpClient;
        private bool _disposed = false;

        public ApiClientService(ILogger<ApiClientService> logger, IOptions<SmartFactoryConfig> config)
        {
            _logger = logger;
            _config = config.Value;
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            
            // Set default headers
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "SmartFactoryClient/1.0");
        }

        /// <summary>
        /// Send sensor data to the Smart Factory API
        /// </summary>
        public async Task<bool> SendSensorDataAsync(string rawSensorData)
        {
            var attempts = 0;
            var maxAttempts = _config.RetryAttempts;

            while (attempts < maxAttempts)
            {
                attempts++;
                
                try
                {
                    _logger.LogDebug("Sending data to API (attempt {Attempt}/{MaxAttempts}): {Data}", 
                        attempts, maxAttempts, rawSensorData.Replace("\n", "\\n"));

                    var url = $"{_config.ApiBaseUrl.TrimEnd('/')}{_config.ApiEndpoint}";
                    
                    // Create the HTTP content
                    var content = new StringContent(rawSensorData, Encoding.UTF8, "text/plain");

                    // Send the request
                    var response = await _httpClient.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        _logger.LogInformation("✅ Data sent successfully to API. Response: {Response}", responseContent);
                        return true;
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogWarning("❌ API request failed with status {StatusCode}: {Error}", 
                            response.StatusCode, errorContent);
                        
                        // Don't retry for client errors (4xx)
                        if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
                        {
                            _logger.LogError("Client error detected, not retrying");
                            return false;
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogWarning(ex, "HTTP request failed (attempt {Attempt}/{MaxAttempts})", attempts, maxAttempts);
                }
                catch (TaskCanceledException ex)
                {
                    _logger.LogWarning(ex, "Request timeout (attempt {Attempt}/{MaxAttempts})", attempts, maxAttempts);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error sending data to API (attempt {Attempt}/{MaxAttempts})", attempts, maxAttempts);
                }

                // Wait before retry (except for the last attempt)
                if (attempts < maxAttempts)
                {
                    var delay = _config.RetryDelay * attempts; // Exponential backoff
                    _logger.LogInformation("⏳ Waiting {Delay}ms before retry...", delay);
                    await Task.Delay(delay);
                }
            }

            _logger.LogError("❌ Failed to send data to API after {MaxAttempts} attempts", maxAttempts);
            return false;
        }

        /// <summary>
        /// Send sensor data object to the API (JSON format)
        /// </summary>
        public async Task<bool> SendSensorDataAsync(SensorData sensorData)
        {
            try
            {
                var json = JsonSerializer.Serialize(new { sensorString = sensorData.RawData });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var url = $"{_config.ApiBaseUrl.TrimEnd('/')}/sensor/data";
                var response = await _httpClient.PostAsync(url, content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("✅ Sensor data object sent successfully: {Response}", responseContent);
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("❌ Failed to send sensor data object: {Error}", errorContent);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending sensor data object to API");
                return false;
            }
        }

        /// <summary>
        /// Test API connectivity
        /// </summary>
        public async Task<bool> TestConnectivityAsync()
        {
            try
            {
                _logger.LogInformation("🔍 Testing API connectivity...");
                
                var url = $"{_config.ApiBaseUrl.TrimEnd('/')}/sensor/current";
                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("✅ API connectivity test successful");
                    return true;
                }
                else
                {
                    _logger.LogWarning("⚠️ API connectivity test failed with status: {StatusCode}", response.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ API connectivity test failed");
                return false;
            }
        }

        /// <summary>
        /// Get current sensor data from API
        /// </summary>
        public async Task<SensorData?> GetCurrentSensorDataAsync()
        {
            try
            {
                var url = $"{_config.ApiBaseUrl.TrimEnd('/')}/sensor/current";
                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<SensorData>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    return apiResponse?.Data;
                }
                else
                {
                    _logger.LogWarning("Failed to get current sensor data: {StatusCode}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current sensor data from API");
                return null;
            }
        }

        /// <summary>
        /// Send a test message to verify API functionality
        /// </summary>
        public async Task<bool> SendTestDataAsync()
        {
            var testData = "Smart Factory Monitoring System - AUTOMATIC MODE - ONLINE\n" +
                          "Furnace_Temp:25.0,Env_Humid:50.0,Light_Level:300,Gas_Methane:0,Gas_CO:0," +
                          "Machine_Sound:400,Tank_Pressure:50,Main_Current:100,Engine_Vibe:10,Input_Voltage:220," +
                          "Conveyor_Dist:100,Water_Leak:50,Flame_Status:0,Gate_Status:0,E_Stop_Button:0,Coolant_Valve:50";

            _logger.LogInformation("📡 Sending test data to API...");
            return await SendSensorDataAsync(testData);
        }

        /// <summary>
        /// Get API health status
        /// </summary>
        public async Task<string> GetApiHealthAsync()
        {
            try
            {
                var isConnected = await TestConnectivityAsync();
                if (isConnected)
                {
                    return $"✅ Connected to {_config.ApiBaseUrl}";
                }
                else
                {
                    return $"❌ Cannot connect to {_config.ApiBaseUrl}";
                }
            }
            catch (Exception ex)
            {
                return $"❌ Error: {ex.Message}";
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _httpClient?.Dispose();
                _disposed = true;
            }
        }
    }
}