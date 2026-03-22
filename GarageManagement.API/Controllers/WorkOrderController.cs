using ComponentManagement.PaginationUtility;
using GarageManagement.Application.DTOs;
using GarageManagement.Application.Interfaces.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GarageManagement.API.Controllers
{
    /// <summary>
    /// Work order API. Tasks are the quotation items of the linked quotation (QuotationId); there is no separate WorkOrderTask table.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class WorkOrderController : ControllerBase
    {
        private readonly IWorkOrderService _workOrderService;

        public WorkOrderController(IWorkOrderService workOrderService)
        {
            _workOrderService = workOrderService;
        }

        /// <summary>
        /// Get work order by ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WorkOrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken = default)
        {
            var result = await _workOrderService.GetByIdAsync(id, cancellationToken);
            if (result == null)
                return NotFound(new { message = $"Work order with ID {id} not found." });
            return Ok(result);
        }

        /// <summary>
        /// Get work orders with pagination.
        /// </summary>
        [HttpPost("paginated")]
        [ProducesResponseType(typeof(PaginationResult<WorkOrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPaginated([FromBody] PaginationRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                return BadRequest(new { message = "Invalid request payload." });
            var result = await _workOrderService.GetPaginatedAsync(request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Create a new work order. Vehicle and quotation must exist. Optional CustomerId is validated if provided.
        /// Created work order has status 'Created'; tasks are the quotation items from the linked quotation.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(WorkOrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateWorkOrderRequestDto request, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _workOrderService.CreateWorkOrderAsync(request, cancellationToken);
                if (result == null)
                    return BadRequest(new { message = "Work order could not be created." });
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
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
        /// Update an existing work order. Status, ScheduledStart, ScheduledEnd, ActualStart, ActualEnd, Notes can be updated.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(long id, [FromBody] WorkOrderDto dto, CancellationToken cancellationToken = default)
        {
            if (dto == null)
                return BadRequest(new { message = "Work order data cannot be null." });
            try
            {
                var result = await _workOrderService.UpdateAsync(id, dto, dto.ModifiedBy);
                if (!result)
                    return NotFound(new { message = $"Work order with ID {id} not found." });
                return Ok(new { success = true, message = "Work order updated successfully." });
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
        /// Soft delete a work order.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken = default)
        {
            var result = await _workOrderService.DeleteAsync(id);
            if (!result)
                return NotFound(new { message = $"Work order with ID {id} not found or already deleted." });
            return Ok(new { success = true, message = "Work order deleted successfully." });
        }
    }
}
