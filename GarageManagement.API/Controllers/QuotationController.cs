using GarageManagement.Application.DTOs;
using GarageManagement.Application.Interfaces.ServiceInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GarageManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotationController : ControllerBase
    {
        private readonly IQuotationService _quotationService;

        public QuotationController(IQuotationService quotationService)
        {
            _quotationService = quotationService;
        }

        /// <summary>
        /// Create a new quotation
        /// </summary>
        [HttpPost("{requestId:long}")]
        public async Task<IActionResult> Create(long requestId, [FromBody] QuotationDTO quotationRequest)
        {
            try
            {
                var result = await _quotationService.CreateQuotationAsync(requestId, quotationRequest);
                return CreatedAtAction(nameof(GetById), new { id = quotationRequest.QuotationID }, quotationRequest);
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

        /// <summary>
        /// Get all quotations
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var quotations = await _quotationService.GetAllQuotationsAsync();
            return Ok(quotations);
        }

        /// <summary>
        /// Get quotation by ID
        /// </summary>
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var quotation = await _quotationService.GetQuotationByIdAsync(id);
            if (quotation == null)
                return NotFound(new { message = $"Quotation with ID {id} not found." });

            return Ok(quotation);
        }

        /// <summary>
        /// Update an existing quotation
        /// </summary>
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] QuotationDTO quotationRequest)
        {
            try
            {
                var result = await _quotationService.UpdateQuotationAsync(id, quotationRequest);
                if (!result)
                    return NotFound(new { message = $"Quotation with ID {id} not found." });

                return Ok(new { success = result });
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

        /// <summary>
        /// Soft delete a quotation
        /// </summary>
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _quotationService.DeleteQuotationAsync(id);
            if (!result)
                return NotFound(new { message = $"Quotation with ID {id} not found or already deleted." });

            return Ok(new { success = result });
        }
    }
}
