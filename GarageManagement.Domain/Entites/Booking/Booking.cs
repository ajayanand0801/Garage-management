namespace GarageManagement.Domain.Entites.Booking
{
    /// <summary>
    /// Booking entity. Maps to [bkg].[Booking].
    /// </summary>
    public class Booking : BaseEntity
    {
        public string? BookingNo { get; set; }
        public Guid BookingGuid { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public long CustomerID { get; set; }
        public long? VehicleID { get; set; }
        public long ServiceID { get; set; }
        public long ServiceRequestID { get; set; }
        public long ObjectType { get; set; }
        public long StatusID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DurationType { get; set; } = "hourly";
        public string? Notes { get; set; }
        public string? MetaData { get; set; }
        public long TenantID { get; set; }
        public long OrgID { get; set; }
        public long DomainID { get; set; }

        public BookingStatus? StatusNavigation { get; set; }
    }
}
