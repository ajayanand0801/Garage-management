using GarageManagement.Application.DTOs;
using GarageManagement.Application.Interfaces.ServiceInterface;
using GarageManagement.Application.Interfaces.Validator;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.Services.ServiceRequest
{
    public class ServiceRequestService : IServiceRequest
    {
        private readonly IJsonValidator _jsonValidator;

        // Assume you inject the schema as well (static or from config)
        private readonly string _jsonSchema = JsonRules.ServiceRequestSchema;

        public ServiceRequestService(IJsonValidator jsonValidator)
        {
            _jsonValidator = jsonValidator;
        }

        public async Task<bool> Create(ServiceRequestDto request)
        {
            // Run schema validation
            if (!_jsonValidator.ValidateJsonPayload(request, _jsonSchema, out List<string> errors))
            {
                // 🔥 Validation failed, map errors
                var errorMessage = string.Join("; ", errors);

                // Throw a custom exception (or return a result object instead)
                throw new ValidationException($"ServiceRequest validation failed: {errorMessage}");
            }

            // If valid, proceed with creation (DB insert or other logic)
            // await _repository.Save(request);
            return true;
        }
    }

}
