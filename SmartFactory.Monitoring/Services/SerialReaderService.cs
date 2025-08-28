using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartFactory.Monitoring.Hubs;
using SmartFactory.Monitoring.Models;

namespace SmartFactory.Monitoring.Services
{
    public class SerialOptions
    {
        public string PortName { get; set; } = "/dev/ttyS0";
        public int BaudRate { get; set; } = 9600;
        public string NewLine { get; set; } = "\n";
    }

    public class SerialReaderService : BackgroundService
    {
        private readonly ILogger<SerialReaderService> _logger;
        private readonly IHubContext<SensorHub> _hub;
        private readonly SerialOptions _options;

        public SerialReaderService(ILogger<SerialReaderService> logger, IHubContext<SensorHub> hub, IOptions<SerialOptions> options)
        {
            _logger = logger;
            _hub = hub;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var serial = new SerialPort(_options.PortName, _options.BaudRate)
                    {
                        NewLine = _options.NewLine,
                        Encoding = Encoding.ASCII,
                        ReadTimeout = 3000
                    };
                    serial.Open();
                    _logger.LogInformation("Serial opened on {Port}", _options.PortName);

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        string line = serial.ReadLine();
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        if (line.Contains(":"))
                        {
                            var payload = SensorPayload.FromKeyValueCsv(line);
                            await _hub.Clients.All.SendAsync("SensorUpdate", payload, cancellationToken: stoppingToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Serial reader error. Reconnecting in 3s...");
                    await Task.Delay(3000, stoppingToken);
                }
            }
        }
    }
}

