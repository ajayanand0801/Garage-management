namespace GarageManagement.Application.DTOs
{
    /// <summary>
    /// Garage service details for booking-with-details response. All properties nullable when entity not found.
    /// </summary>
    public class BookingServiceDetailDto
    {
        public long Id { get; set; }
        public string? Code { get; set; }
        public string? ServiceName { get; set; }
        public long? CategoryId { get; set; }
        public long? ParentId { get; set; }
        public decimal? EstimatedCost { get; set; }
        public bool QuotationRequired { get; set; }
        public string? Description { get; set; }
    }
}
