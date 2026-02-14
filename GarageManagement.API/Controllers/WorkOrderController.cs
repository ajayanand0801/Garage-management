using GarageManagement.Application.DTOs;
using GarageManagement.Application.Interfaces.ServiceInterface;
using Microsoft.AspNetCore.Mvc;

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
        /// Create a new work order. Vehicle and quotation must exist. Optional CustomerId is validated if provided.
        /// Created work order has status 'Created'; tasks are the quotation items from the linked quotation.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(WorkOrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateWorkOrderRequestDto request, CancellationToken cancellationToken)
        {
            if (request == null)
                return BadRequest(new { message = "Request body cannot be null." });

            if (request.VehicleId <= 0)
                return BadRequest(new { message = "VehicleId is required and must be greater than zero." });

            if (request.QuotationId <= 0)
                return BadRequest(new { message = "QuotationId is required and must be greater than zero." });

            if (request.CustomerId.HasValue && request.CustomerId.Value <= 0)
                return BadRequest(new { message = "CustomerId, when provided, must be greater than zero." });

            var result = await _workOrderService.CreateWorkOrderAsync(request, cancellationToken);
            if (result == null)
                return BadRequest(new { message = "Work order could not be created. Ensure customer (if provided), vehicle, and quotation exist." });

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Get work order by ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WorkOrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
        {
            var result = await _workOrderService.GetByIdAsync(id, cancellationToken);
            if (result == null)
                return NotFound(new { message = $"Work order with ID {id} not found." });
            return Ok(result);
        }
    }
}
