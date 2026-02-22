namespace GarageManagement.Application.DTOs
{
    /// <summary>
    /// Booking response with optional customer, vehicle, and service details. Rest payload same as BookingDto.
    /// </summary>
    public class BookingDetailsDto
    {
        public long? Id { get; set; }
        public string? BookingNo { get; set; }
        public Guid? BookingGuid { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public long ServiceRequestId { get; set; }
        public long CustomerID { get; set; }
        public long? VehicleID { get; set; }
        public long ServiceID { get; set; }
        public long StatusID { get; set; }
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

        /// <summary>Populated from Customer table when found; null otherwise.</summary>
        public BookingCustomerDetailDto? CustomerDetails { get; set; }
        /// <summary>Populated from Vehicle table when found; null otherwise.</summary>
        public BookingVehicleDetailDto? VehicleDetails { get; set; }
        /// <summary>Populated from GarageServices table when found; null otherwise.</summary>
        public BookingServiceDetailDto? ServiceDetails { get; set; }
    }
}
