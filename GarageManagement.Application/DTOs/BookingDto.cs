namespace GarageManagement.Application.DTOs
{
    /// <summary>
    /// DTO for booking create, update, and response. ServiceRequestId is required when creating/updating a booking linked to a service request.
    /// </summary>
    public class BookingDto
    {
        public long? Id { get; set; }
        public string? BookingNo { get; set; }
        public Guid? BookingGuid { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        /// <summary>Service request to which this booking is linked. Required on create/update.</summary>
        public long ServiceRequestId { get; set; }
        public long CustomerID { get; set; }
        public long? VehicleID { get; set; }
        public long ServiceID { get; set; }
        public long StatusID { get; set; }
        /// <summary>Status name from [bkg].[BookingStatus] (populated on read).</summary>
        public string? StatusName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? DurationType { get; set; }
        public string? Notes { get; set; }
        public string? MetaData { get; set; }
        public long TenantID { get; set; }
        public long OrgID { get; set; }
        public long DomainID { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
    }
}
