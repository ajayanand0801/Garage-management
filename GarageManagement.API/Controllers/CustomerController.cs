using ComponentManagement.PaginationUtility;
using GarageManagement.Application.DTOs;
using GarageManagement.Application.Interfaces.ServiceInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GarageManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Get all customers with pagination
        /// </summary>
        [HttpPost("paginated")]
        public async Task<IActionResult> GetAllCustomers([FromBody] PaginationRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
                return BadRequest("Invalid request payload.");

            var paginatedResult = await _customerService.GetAllCustomersAsync(request, cancellationToken);

            return Ok(paginatedResult);
        }

        /// <summary>
        /// Get all customers (without pagination)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        /// <summary>
        /// Get customer by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return NotFound(new { message = $"Customer with ID {id} not found." });

            return Ok(customer);
        }

        /// <summary>
        /// Create a new customer
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrmCustomerDTO customerDto)
        {
            if (customerDto == null)
                return BadRequest("Customer data cannot be null.");

            var createdCustomer = await _customerService.CreateCustomerAsync(customerDto);
            if (createdCustomer == null || !createdCustomer.Id.HasValue)
                return BadRequest("Failed to create customer.");

            return CreatedAtAction(nameof(GetById), new { id = createdCustomer.Id.Value }, createdCustomer);
        }

        /// <summary>
        /// Update an existing customer
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] CrmCustomerDTO customerDto)
        {
            if (customerDto == null)
                return BadRequest("Customer data cannot be null.");

            var result = await _customerService.UpdateCustomerAsync(id, customerDto);
            if (!result)
                return NotFound(new { message = $"Customer with ID {id} not found." });

            return Ok(new { success = true, message = "Customer updated successfully." });
        }

        /// <summary>
        /// Soft delete a customer
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);
            if (!result)
                return NotFound(new { message = $"Customer with ID {id} not found or already deleted." });

            return Ok(new { success = true, message = "Customer deleted successfully." });
        }
    }
}

