using System.ComponentModel.DataAnnotations;

namespace GarageManagement.Application.DTOs
{
    /// <summary>
    /// Request to update quotation status. Only Approved or Rejected allowed; RejectionNotes required when Rejected.
    /// </summary>
    public class UpdateQuotationStatusRequest
    {
        /// <summary>
        /// New status. Allowed values: Approved, Rejected (Draft is not allowed for this endpoint).
        /// </summary>
        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; } = null!;

        /// <summary>
        /// Required when Status is Rejected. Notes describing the reason for rejection.
        /// </summary>
        public string? RejectionNotes { get; set; }
    }
}
