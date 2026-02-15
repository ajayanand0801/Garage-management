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
