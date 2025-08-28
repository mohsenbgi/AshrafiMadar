using System.Collections.Generic;

namespace SmartFactory.Monitoring.Models
{
    public class SensorPayload : Dictionary<string, string>
    {
        public static SensorPayload FromKeyValueCsv(string input)
        {
            var payload = new SensorPayload();
            if (string.IsNullOrWhiteSpace(input)) return payload;
            var parts = input.Split(',', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);
            foreach (var part in parts)
            {
                var kv = part.Split(':', 2);
                if (kv.Length == 2)
                {
                    payload[kv[0]] = kv[1];
                }
            }
            return payload;
        }
    }
}

