namespace GarageManagement.Application.DTOs
{
    /// <summary>
    /// Request DTO for creating a work order.
    /// CustomerId is optional and used for validation only (customer must exist if provided).
    /// Tasks are the quotation items from the linked quotation (QuotationId).
    /// </summary>
    public class CreateWorkOrderRequestDto
    {
        public long VehicleId { get; set; }
        public long QuotationId { get; set; }
        /// <summary>Optional; used to validate that the customer exists.</summary>
        public long? CustomerId { get; set; }
        public DateTime? ScheduledStart { get; set; }
        public DateTime? ScheduledEnd { get; set; }
        public string? Notes { get; set; }
    }
}
