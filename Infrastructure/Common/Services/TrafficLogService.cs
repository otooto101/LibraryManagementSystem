using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.RegularExpressions;

namespace Infrastructure.Services
{
    public class TrafficLogService : ITrafficLogService
    {
        private readonly string[] _sensitiveKeys = ["password", "token", "secret", "creditcard"];
        private readonly string _baseLogPath;
        private readonly bool _logBody;

        public TrafficLogService(IConfiguration config)
        {
            var settings = config.GetSection("RequestLogging");

            string folderName = settings.GetValue<string>("StoragePath") ?? "Traffic";

            // Set base path: Logs/2026-02-06/Traffic_Dev/
            _baseLogPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", DateTime.Now.ToString("yyyy-MM-dd"), folderName);

            _logBody = settings.GetValue<bool>("LogBody", true);
        }

        public async Task SaveLogAsync(Stream bodyStream, string contentType, string traceId, string method, string path, string typeSuffix)
        {
            string body;

            if (_logBody)
            {
                body = await ReadStreamAsync(bodyStream);

                if (string.IsNullOrWhiteSpace(body) || contentType?.Contains("json", StringComparison.OrdinalIgnoreCase) != true)
                {
                    body = "{}";
                }

                body = MaskSensitiveData(body);
            }
            else
            {
                body = "\"(Body Logging Disabled)\"";
            }

            var envelope = $$"""
            {
              "Meta": {
                "Timestamp": "{{DateTime.Now:yyyy-MM-dd HH:mm:ss}}",
                "Method": "{{method}}",
                "Path": "{{path}}",
                "TraceId": "{{traceId}}"
              },
              "Body": {{body}}
            }
            """;

            var safeTraceId = traceId.Replace(":", "-");
            var caseFolder = $"{DateTime.Now:HH-mm-ss}_{safeTraceId}";

            var fullFolderPath = Path.Combine(_baseLogPath, caseFolder);
            var fileName = typeSuffix == "Req" ? "Request.json" : "Response.json";

            if (!Directory.Exists(fullFolderPath)) Directory.CreateDirectory(fullFolderPath);

            await File.WriteAllTextAsync(Path.Combine(fullFolderPath, fileName), envelope);
        }

        private static async Task<string> ReadStreamAsync(Stream stream)
        {
            if (!stream.CanSeek) return "{}";
            stream.Position = 0;
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            var text = await reader.ReadToEndAsync();
            stream.Position = 0;
            return text;
        }

        private string MaskSensitiveData(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return json;
            foreach (var key in _sensitiveKeys)
            {
                var pattern = $"(\"{key}\"\\s*:\\s*\")([^\"]+)(\")";
                json = Regex.Replace(json, pattern, "$1***MASKED***$3", RegexOptions.IgnoreCase);
            }
            return json;
        }
    }
}