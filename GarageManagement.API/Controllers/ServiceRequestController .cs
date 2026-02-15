using ComponentManagement.PaginationUtility;
using GarageManagement.Application.DTOs;
using GarageManagement.Application.Interfaces.ServiceInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GarageManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceRequestController : ControllerBase
    {
        private readonly IServiceRequest _serviceRequest;

        public ServiceRequestController(IServiceRequest serviceRequest)
        {
            _serviceRequest = serviceRequest;
        }

        /// <summary>
        /// Get paginated list of service requests.
        /// </summary>
        [HttpPost("paginated")]
        public async Task<IActionResult> GetServiceRequests([FromBody] PaginationRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
                return BadRequest("Invalid request payload.");

            var paginatedResult = await _serviceRequest.GetServiceRequestsAsync(request, cancellationToken);
            return Ok(paginatedResult);
        }

        /// <summary>
        /// Get service request by ID with full details (customer, vehicle, documents, metadata).
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _serviceRequest.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = $"Service request with ID {id} not found." });

            return Ok(result);
        }

        /// <summary>
        /// Soft delete a service request (sets IsActive = 0).
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _serviceRequest.DeleteServiceRequestAsync(id);
            if (!result)
                return NotFound(new { message = $"Service request with ID {id} not found or already deleted." });

            return Ok(new { success = true, message = "Service request deleted successfully." });
        }

        /// <summary>
        /// Update service request by ID. Updates vehicle and customer sections and all related tables (ServiceRequest, SRCustomerMetaData, SRVehicleMetaData, ServiceRequestMetadata).
        /// </summary>
        [HttpPut("{serviceRequestId}")]
        public async Task<IActionResult> Update(long serviceRequestId, [FromBody] ServiceRequestDto request, [FromQuery] string? modifiedBy = null)
        {
            if (request == null)
                return BadRequest("Invalid request payload.");

            var updated = await _serviceRequest.UpdateByServiceRequestId(serviceRequestId, request, modifiedBy);
            if (!updated)
                return NotFound(new { message = "Service request not found or could not be updated." });

            return Ok(new { success = true, serviceRequestId });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServiceRequestDto request)
        {
            try
            {
                var result = await _serviceRequest.Create(request);
                return CreatedAtAction(nameof(Create), new { success = result }, request);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new
                {
                    message = "Validation failed",
                    errors = ex.Message.Split(';', StringSplitOptions.TrimEntries)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred.",
                    error = ex.Message
                });
            }
        }
    }
}
