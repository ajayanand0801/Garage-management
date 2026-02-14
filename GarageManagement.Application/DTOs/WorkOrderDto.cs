namespace GarageManagement.Application.DTOs
{
    /// <summary>
    /// Response DTO for a work order.
    /// </summary>
    public class WorkOrderDto
    {
        public long Id { get; set; }
        public Guid OrderGuid { get; set; }
        public long VehicleId { get; set; }
        public long QuotationId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime? ScheduledStart { get; set; }
        public DateTime? ScheduledEnd { get; set; }
        public DateTime? ActualStart { get; set; }
        public DateTime? ActualEnd { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
