using ComponentManagement.PaginationUtility;
using GarageManagement.Application.DTOs;
using GarageManagement.Application.Interfaces.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GarageManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Get booking by ID with customer, vehicle, and service details. Detail objects are null when not found.
        /// </summary>
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetByIdWithDetails(long id)
        {
            var booking = await _bookingService.GetByIdWithDetailsAsync(id);
            if (booking == null)
                return NotFound(new { message = $"Booking with ID {id} not found." });
            return Ok(booking);
        }

        /// <summary>
        /// Get booking by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null)
                return NotFound(new { message = $"Booking with ID {id} not found." });
            return Ok(booking);
        }

        /// <summary>
        /// Get bookings with pagination
        /// </summary>
        [HttpPost("paginated")]
        public async Task<IActionResult> GetPaginated([FromBody] PaginationRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
                return BadRequest("Invalid request payload.");
            var result = await _bookingService.GetPaginatedAsync(request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Create a new booking. ServiceRequestId, CustomerID are required; VehicleID must match Service Request when provided.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookingDto dto)
        {
            if (dto == null)
                return BadRequest("Booking data cannot be null.");
            try
            {
                var created = await _bookingService.CreateAsync(dto, dto.CreatedBy);
                if (created == null || !created.Id.HasValue)
                    return BadRequest("Failed to create booking.");
                return CreatedAtAction(nameof(GetById), new { id = created.Id.Value }, created);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing booking. CustomerID, VehicleID, ServiceRequestId must exist, be active, and match.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] BookingDto dto)
        {
            if (dto == null)
                return BadRequest("Booking data cannot be null.");
            try
            {
                var result = await _bookingService.UpdateAsync(id, dto, dto.ModifiedBy);
                if (!result)
                    return NotFound(new { message = $"Booking with ID {id} not found." });
                return Ok(new { success = true, message = "Booking updated successfully." });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Partial update of a booking by ID. Updates only provided fields: StatusID, Status, Type, StartDate, EndDate, DurationType, Notes.
        /// StartDate and EndDate must not be in the past. StatusID must be a valid active [bkg].[BookingStatus] Id.
        /// </summary>
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(long id, [FromBody] BookingPatchDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Patch payload cannot be null." });
            try
            {
                var updated = await _bookingService.PatchAsync(id, dto);
                if (updated == null)
                    return NotFound(new { message = $"Booking with ID {id} not found." });
                return Ok(updated);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Soft delete a booking
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _bookingService.DeleteAsync(id);
            if (!result)
                return NotFound(new { message = $"Booking with ID {id} not found or already deleted." });
            return Ok(new { success = true, message = "Booking deleted successfully." });
        }
    }
}
