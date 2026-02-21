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
        /// Example: GET api/lookup?type=GarageService
        /// </summary>
       [ HttpGet("garage-services")]
        public async Task<IActionResult> GetLookup([FromQuery] LookupType type, CancellationToken cancellationToken)
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
    }
}
