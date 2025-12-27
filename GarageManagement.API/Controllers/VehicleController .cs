using GarageManagement.Application.DTOs;
using GarageManagement.Application.Interfaces.ServiceInterface;
using GarageManagement.Domain.Entites.Vehicles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GarageManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _service;

        public VehicleController(IVehicleService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllVehiclesAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetVehicleByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VehicleDto vehicle)
        {
            var result = await _service.CreateVehicle(vehicle);
            return result ? Ok() : BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] Vehicle vehicle)
        {
            var result = await _service.UpdateVehicleAsync(id, vehicle);
            return result ? Ok() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _service.DeleteVehicleAsync(id);
            return result ? Ok() : NotFound();
        }

        [HttpGet("details/{vin}")]
        public async Task<IActionResult> VehicleDetailsByVin(string vin)
        {
            var result = await _service.GetVehicleBYVin(vin);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut("{vehicleId}/owners")]
        public async Task<IActionResult> UpdateVehicleOwners(long vehicleId, [FromBody] List<VehicleOwnerDto> owners)
        {
            if (owners == null || owners.Count == 0)
                return BadRequest("Owners list cannot be null or empty");

            var result = await _service.UpdateVehicleOwnersAsync(vehicleId, owners);
            return result ? Ok() : NotFound();
        }
    }

}
