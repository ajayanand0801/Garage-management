using GarageManagement.Application.Interfaces.Validator;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Infrastructure.Validator
{
    public class JsonValidator : IJsonValidator
    {
        public bool Validate<T>(T requestPayload, string jsonRule, out List<string> errors)
        {
            errors = new List<string>();

            JObject jsonNode;
            try
            {
                jsonNode = JObject.FromObject(requestPayload);
            }
            catch (Exception ex)
            {
                errors.Add("Invalid payload: cannot convert to JSON. " + ex.Message);
                return false;
            }

            JObject ruleNode;
            try
            {
                ruleNode = JObject.Parse(jsonRule);
            }
            catch (Exception ex)
            {
                errors.Add($"Invalid rule JSON: {ex.Message}");
                return false;
            }

            // You will need to rewrite ValidateNode to use JToken/JObject instead of JsonNode,
            // or simply rely on ValidateJsonPayload method below.

            return ValidateJsonPayload(requestPayload, jsonRule, out errors);
        }

        public bool ValidateJsonPayload<T>(T requestPayload, string jsonRule, out List<string> errors)
        {
            errors = new List<string>();

            try
            {
                JSchema schema = JSchema.Parse(jsonRule);
                string jsonData = JsonConvert.SerializeObject(requestPayload);
                JObject jsonObject = JObject.Parse(jsonData);

                bool isValid = jsonObject.IsValid(schema, out IList<string> validationErrors);

                if (!isValid)
                {
                    errors.AddRange(validationErrors);
                }

                return isValid;
            }
            catch (Exception ex)
            {
                errors.Add($"Validation exception: {ex.Message}");
                return false;
            }
        }
    }
}
