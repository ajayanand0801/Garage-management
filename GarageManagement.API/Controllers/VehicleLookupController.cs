using GarageManagement.Application.DTOs;
using GarageManagement.Application.Interfaces.ServiceInterface;
using GarageManagement.Domain.Entites.Vehicles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GarageManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  

    public class VehicleLookupController : ControllerBase
    {
        private readonly IVehicleLookupService _service;

        public VehicleLookupController(IVehicleLookupService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllVehicleLookupsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetVehicleLookupByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("by-type/{lookupType}")]
        public async Task<IActionResult> GetByType(string lookupType)
        {
            var result = await _service.GetVehicleLookupsByTypeAsync(lookupType);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VehicleLookupDTO lookupDto)
        {
            var result = await _service.CreateVehicleLookupAsync(lookupDto);
            return result ? Ok() : BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] VehicleLookupDTO lookupDto)
        {
            var result = await _service.UpdateVehicleLookupAsync(id, lookupDto);
            return result ? Ok() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteVehicleLookupAsync(id);
            return result ? Ok() : NotFound();
        }
    }


}
