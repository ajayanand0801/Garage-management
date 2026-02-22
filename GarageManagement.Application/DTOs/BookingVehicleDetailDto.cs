namespace GarageManagement.Application.DTOs
{
    /// <summary>
    /// Vehicle details for booking-with-details response. All properties nullable when entity not found.
    /// </summary>
    public class BookingVehicleDetailDto
    {
        public long Id { get; set; }
        public long VehicleID { get; set; }
        public string? VIN { get; set; }
        public string? Color { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? EngineNumber { get; set; }
        public string? ChassisNumber { get; set; }
        public long? Mileage { get; set; }
        public decimal? EngineSize { get; set; }
        public DateTime? RegDate { get; set; }
        public DateTime? ManufactureDate { get; set; }
    }
}
