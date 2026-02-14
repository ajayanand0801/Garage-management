using System;

namespace GarageManagement.Domain.Entites.WorkOrder
{
    /// <summary>
    /// Work order entity for tracking vehicle service work.
    /// Tasks are represented by the linked quotation's items (via QuotationId).
    /// </summary>
    public class WorkOrder : BaseEntity
    {
        public Guid OrderGuid { get; set; }
        public long TenantID { get; set; }
        public int OrgID { get; set; }
        public long VehicleId { get; set; }
        public long QuotationId { get; set; }
        public string Status { get; set; } = "Created";
        public DateTime? ScheduledStart { get; set; }
        public DateTime? ScheduledEnd { get; set; }
        public DateTime? ActualStart { get; set; }
        public DateTime? ActualEnd { get; set; }
        public string? Notes { get; set; }
    }
}
