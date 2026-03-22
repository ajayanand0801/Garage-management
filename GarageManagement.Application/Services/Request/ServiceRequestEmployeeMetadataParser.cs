using GarageManagement.Domain.Entites.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace GarageManagement.Application.Services.Request
{
    /// <summary>
    /// Reads <see cref="ServiceRequestMetadata"/> with KeyName "Employee" as a JSON object of key/value pairs.
    /// </summary>
    internal static class ServiceRequestEmployeeMetadataParser
    {
        public const string EmployeeMetadataKeyName = "Employee";

        public static Dictionary<string, string>? TryParseFromEntries(ICollection<ServiceRequestMetadata>? entries)
        {
            var meta = entries?.FirstOrDefault(m =>
                string.Equals(m.KeyName, EmployeeMetadataKeyName, StringComparison.OrdinalIgnoreCase));
            if (meta == null || string.IsNullOrWhiteSpace(meta.KeyValue))
                return null;
            return TryParseKeyValuesJson(meta.KeyValue);
        }

        public static Dictionary<string, string>? TryParseKeyValuesJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                var direct = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (direct != null && direct.Count > 0)
                    return direct;
            }
            catch (JsonException)
            {
                // fall through to JsonDocument
            }

            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.ValueKind != JsonValueKind.Object)
                    return null;

                var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    dict[prop.Name] = JsonElementToString(prop.Value);
                }

                return dict.Count > 0 ? dict : null;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private static string JsonElementToString(JsonElement el) =>
            el.ValueKind switch
            {
                JsonValueKind.String => el.GetString() ?? string.Empty,
                JsonValueKind.Number => el.GetRawText(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                JsonValueKind.Null => string.Empty,
                _ => el.GetRawText()
            };
    }
}
