using GarageManagement.Application.DTOs;
using GarageManagement.Application.Enums;
using GarageManagement.Application.Interfaces.ServiceInterface;
using Microsoft.AspNetCore.Mvc;

namespace GarageManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupController : ControllerBase
    {
        private readonly ILookupService _lookupService;

        public LookupController(ILookupService lookupService)
        {
            _lookupService = lookupService;
        }

        /// <summary>
        /// Get lookup data by type. Returns standard payload: Id, Code, DisplayName.
        /// Types: GarageService (1), BookingStatus (2), ServiceCategory (3).
        /// Example: GET api/lookup?type=GarageService | GET api/lookup?type=BookingStatus | GET api/lookup?type=ServiceCategory
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetLookup([FromQuery] LookupType type, CancellationToken cancellationToken = default)
        {
            try
            {
                var items = await _lookupService.GetLookupAsync(type, cancellationToken);
                return Ok(items);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get lookup data by type name (string). Accepts: GarageService, BookingStatus, ServiceCategory, or Type (returns ServiceCategory).
        /// Case-insensitive. Returns standard payload: Id, Code, DisplayName.
        /// Example: GET api/lookup/by-name?typeName=Type | GET api/lookup/by-name?typeName=BookingStatus
        /// </summary>
        [HttpGet("type")]
        public async Task<IActionResult> GetLookupByTypeName([FromQuery] string typeName, CancellationToken cancellationToken = default)
        {
            try
            {
                var items = await _lookupService.GetLookupByTypeNameAsync(typeName, cancellationToken);
                return Ok(items);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
