using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace GarageManagement.Shared.Utilities
{
    public static class JsonValidator
    {
        public static bool Validate<T>(T requestPayload, string jsonRule, out List<string> errors)
        {
            errors = new List<string>();

            // Serialize requestPayload to JsonNode for generic access
            JsonNode jsonNode = JsonSerializer.SerializeToNode(requestPayload);

            if (jsonNode == null)
            {
                errors.Add("Invalid payload: cannot convert to JSON.");
                return false;
            }

            // Parse rules JSON
            JsonNode ruleNode;
            try
            {
                ruleNode = JsonNode.Parse(jsonRule);
            }
            catch (Exception ex)
            {
                errors.Add($"Invalid rule JSON: {ex.Message}");
                return false;
            }

            // Validate recursively
            ValidateNode(jsonNode, ruleNode, "", errors);

            return errors.Count == 0;
        }

        private static void ValidateNode(JsonNode dataNode, JsonNode ruleNode, string path, List<string> errors)
        {
            if (ruleNode == null)
                return;

            foreach (var prop in ruleNode.AsObject())
            {
                string propName = prop.Key;
                JsonNode propRule = prop.Value;

                string currentPath = string.IsNullOrEmpty(path) ? propName : $"{path}.{propName}";

                bool isRequired = propRule["required"]?.GetValue<bool>() ?? false;
                bool isNullable = propRule["nullable"]?.GetValue<bool>() ?? false;
                string type = propRule["type"]?.GetValue<string>();

                JsonNode? valueNode = null;

                if (dataNode is JsonObject obj)
                    obj.TryGetPropertyValue(propName, out valueNode);
                else if (dataNode is JsonArray arr && int.TryParse(propName, out int index) && index < arr.Count)
                    valueNode = arr[index];

                // Check required
                if (isRequired && valueNode == null)
                {
                    errors.Add($"Missing required field '{currentPath}'");
                    continue;
                }

                // If not required and not present, skip further validation
                if (valueNode == null)
                    continue;

                // Check nullability
                if (!isNullable && valueNode.GetValue<object>() == null)
                {
                    errors.Add($"Field '{currentPath}' cannot be null");
                    continue;
                }

                // Validate by type
                if (!ValidateType(valueNode, type))
                {
                    errors.Add($"Field '{currentPath}' expected type '{type}', but got '{GetNodeTypeName(valueNode)}'");
                    continue;
                }

                // If type is object, recurse into its properties
                if (type == "object" && propRule["properties"] != null)
                {
                    ValidateNode(valueNode, propRule["properties"], currentPath, errors);
                }

                // If type is array, validate each item
                if (type == "array" && propRule["items"] != null && valueNode is JsonArray arrNode)
                {
                    for (int i = 0; i < arrNode.Count; i++)
                    {
                        ValidateNode(arrNode[i], propRule["items"], $"{currentPath}[{i}]", errors);
                    }
                }
            }
        }

        private static bool ValidateType(JsonNode node, string expectedType)
        {
            if (node == null)
                return false;

            var value = node.GetValue<object>();

            if (value == null)
                return true; // null allowed if nullable, checked before

            switch (expectedType)
            {
                case "int":
                    return value is int || value is long || (value is JsonElement je && je.ValueKind == JsonValueKind.Number && je.TryGetInt64(out _));
                case "string":
                    return value is string;
                case "bool":
                    return value is bool;
                case "datetime":
                    if (value is DateTime) return true;
                    if (value is string s)
                    {
                        return DateTime.TryParse(s, out _);
                    }
                    if (value is JsonElement element && element.ValueKind == JsonValueKind.String)
                    {
                        return DateTime.TryParse(element.GetString(), out _);
                    }
                    return false;
                case "object":
                    return node is JsonObject;
                case "array":
                    return node is JsonArray;
                default:
                    return false;
            }
        }

        private static string GetNodeTypeName(JsonNode node)
        {
            if (node == null) return "null";

            var value = node.GetValue<object>();
            if (value == null) return "null";

            if (value is int || value is long) return "int";
            if (value is string) return "string";
            if (value is bool) return "bool";
            if (value is JsonObject) return "object";
            if (value is JsonArray) return "array";
            if (value is DateTime) return "datetime";

            return value.GetType().Name;
        }
    }

}
