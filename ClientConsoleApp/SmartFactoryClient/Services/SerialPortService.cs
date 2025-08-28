using SmartFactoryClient.Models;
using System.IO.Ports;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SmartFactoryClient.Services
{
    /// <summary>
    /// Service for handling serial port communication with Proteus
    /// </summary>
    public class SerialPortService : IDisposable
    {
        private readonly ILogger<SerialPortService> _logger;
        private readonly SerialPortConfig _config;
        private SerialPort? _serialPort;
        private readonly StringBuilder _dataBuffer;
        private bool _disposed = false;

        public event EventHandler<string>? DataReceived;

        public SerialPortService(ILogger<SerialPortService> logger, IOptions<SerialPortConfig> config)
        {
            _logger = logger;
            _config = config.Value;
            _dataBuffer = new StringBuilder();
        }

        /// <summary>
        /// Initialize and open the serial port connection
        /// </summary>
        public async Task<bool> InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Initializing serial port connection...");
                
                // Check if port exists
                var availablePorts = SerialPort.GetPortNames();
                _logger.LogInformation("Available COM ports: {Ports}", string.Join(", ", availablePorts));

                if (!availablePorts.Contains(_config.PortName))
                {
                    _logger.LogWarning("Port {PortName} not found. Available ports: {AvailablePorts}", 
                        _config.PortName, string.Join(", ", availablePorts));
                    
                    // Try to use the first available port if configured port is not found
                    if (availablePorts.Length > 0)
                    {
                        _config.PortName = availablePorts[0];
                        _logger.LogInformation("Using port {PortName} instead", _config.PortName);
                    }
                    else
                    {
                        _logger.LogError("No COM ports available");
                        return false;
                    }
                }

                _serialPort = new SerialPort
                {
                    PortName = _config.PortName,
                    BaudRate = _config.BaudRate,
                    Parity = Enum.Parse<Parity>(_config.Parity),
                    DataBits = _config.DataBits,
                    StopBits = Enum.Parse<StopBits>(_config.StopBits),
                    Handshake = Enum.Parse<Handshake>(_config.Handshake),
                    ReadTimeout = _config.ReadTimeout,
                    WriteTimeout = _config.WriteTimeout,
                    Encoding = Encoding.UTF8
                };

                // Subscribe to data received event
                _serialPort.DataReceived += OnDataReceived;
                _serialPort.ErrorReceived += OnErrorReceived;

                // Open the port
                _serialPort.Open();
                
                _logger.LogInformation("Serial port {PortName} opened successfully at {BaudRate} baud", 
                    _config.PortName, _config.BaudRate);

                // Test the connection
                await Task.Delay(1000); // Give it a moment to stabilize
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize serial port {PortName}", _config.PortName);
                return false;
            }
        }

        /// <summary>
        /// Handle incoming data from serial port
        /// </summary>
        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (_serialPort == null || !_serialPort.IsOpen)
                    return;

                // Read available data
                string incomingData = _serialPort.ReadLine();
                
                if (string.IsNullOrEmpty(incomingData))
                    return;

                DataReceived?.Invoke(this, incomingData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing received data");
            }
        }

        /// <summary>
        /// Handle serial port errors
        /// </summary>
        private void OnErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            _logger.LogError("Serial port error: {Error}", e.EventType);
        }

        /// <summary>
        /// Send data to the serial port (for testing purposes)
        /// </summary>
        public async Task<bool> SendDataAsync(string data)
        {
            try
            {
                if (_serialPort == null || !_serialPort.IsOpen)
                {
                    _logger.LogWarning("Serial port is not open");
                    return false;
                }

                await Task.Run(() => _serialPort.Write(data));
                _logger.LogDebug("Sent data: {Data}", data);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending data to serial port");
                return false;
            }
        }

        /// <summary>
        /// Close the serial port connection
        /// </summary>
        public void Close()
        {
            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.DataReceived -= OnDataReceived;
                    _serialPort.ErrorReceived -= OnErrorReceived;
                    _serialPort.Close();
                    _logger.LogInformation("Serial port closed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing serial port");
            }
        }

        /// <summary>
        /// Check if the serial port is open and ready
        /// </summary>
        public bool IsConnected => _serialPort?.IsOpen == true;

        /// <summary>
        /// Get connection status information
        /// </summary>
        public string GetConnectionStatus()
        {
            if (_serialPort == null)
                return "Not initialized";
            
            if (_serialPort.IsOpen)
                return $"Connected to {_serialPort.PortName} at {_serialPort.BaudRate} baud";
            
            return "Disconnected";
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Close();
                _serialPort?.Dispose();
                _disposed = true;
            }
        }
    }
}