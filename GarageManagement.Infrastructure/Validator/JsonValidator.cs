using GarageManagement.Application.Interfaces.Validator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace GarageManagement.Infrastructure.Validator
{
    public class JsonValidator : IJsonValidator
    {
        public bool Validate<T>(T requestPayload, string jsonRule, out List<string> errors)
        {
            errors = new List<string>();

            JsonNode jsonNode = JsonSerializer.SerializeToNode(requestPayload);

            if (jsonNode == null)
            {
                errors.Add("Invalid payload: cannot convert to JSON.");
                return false;
            }

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

            ValidateNode(jsonNode, ruleNode, "", errors);

            return errors.Count == 0;
        }

        private void ValidateNode(JsonNode? dataNode, JsonNode? ruleNode, string path, List<string> errors)
        {
            if (ruleNode == null || dataNode == null)
                return;

            foreach (var prop in ruleNode.AsObject())
            {
                string propName = prop.Key;
                JsonNode propRule = prop.Value!;
                string currentPath = string.IsNullOrEmpty(path) ? propName : $"{path}.{propName}";

                // Extract rule attributes
                bool isRequired = propRule["required"]?.GetValue<bool>() ?? false;
                bool isNullable = propRule["nullable"]?.GetValue<bool>() ?? false;
                string type = propRule["type"]?.GetValue<string>() ?? "object";

                JsonNode? valueNode = null;

                // Case-insensitive key lookup
                if (dataNode is JsonObject obj)
                {
                    var match = obj.FirstOrDefault(kvp => string.Equals(kvp.Key, propName, StringComparison.OrdinalIgnoreCase));
                    valueNode = match.Value;
                }
                else if (dataNode is JsonArray arr && int.TryParse(propName, out int index) && index < arr.Count)
                {
                    valueNode = arr[index];
                }

                // Required field check
                if (isRequired && valueNode == null)
                {
                    errors.Add($"Missing required field '{currentPath}'");
                    continue;
                }

                if (valueNode == null)
                    continue;

                // Nullability check
                if (!isNullable && valueNode is JsonValue val && val.TryGetValue<object>(out var objValue) && objValue == null)
                {
                    errors.Add($"Field '{currentPath}' cannot be null");
                    continue;
                }

                //if (!isNullable && valueNode.GetValue<object?>() == null)
                //{
                //    errors.Add($"Field '{currentPath}' cannot be null");
                //    continue;
                //}

                // Type check
                if (!ValidateType(valueNode, type))
                {
                    errors.Add($"Field '{currentPath}' expected type '{type}', but got '{GetNodeTypeName(valueNode)}'");
                    continue;
                }

                // Recursive for nested objects
                if (type == "object" && propRule["properties"] is JsonNode nestedRules)
                {
                    ValidateNode(valueNode, nestedRules, currentPath, errors);
                }

                // Recursive for arrays
                if (type == "array" && propRule["items"] is JsonNode itemRules && valueNode is JsonArray jsonArray)
                {
                    for (int i = 0; i < jsonArray.Count; i++)
                    {
                        ValidateNode(jsonArray[i], itemRules, $"{currentPath}[{i}]", errors);
                    }
                }
            }
        }


        private bool ValidateType(JsonNode valueNode, string expectedType)
        {
            return expectedType switch
            {
                "string" => valueNode is JsonValue v && v.TryGetValue<string>(out _),
                "int" => valueNode is JsonValue v && v.TryGetValue<int>(out _),
                "bool" => valueNode is JsonValue v && v.TryGetValue<bool>(out _),
                "datetime" => valueNode is JsonValue v && v.TryGetValue<DateTime>(out _),
                "object" => valueNode is JsonObject,
                "array" => valueNode is JsonArray,
                _ => false
            };
        }


        private string GetNodeTypeName(JsonNode node)
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
