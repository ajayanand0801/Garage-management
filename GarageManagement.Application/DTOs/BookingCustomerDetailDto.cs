namespace GarageManagement.Application.DTOs
{
    /// <summary>
    /// Customer details for booking-with-details response. All properties nullable when entity not found.
    /// </summary>
    public class BookingCustomerDetailDto
    {
        public long Id { get; set; }
        public string? CustomerType { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? MobileNo { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? CountryCode { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? CompanyName { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPhone { get; set; }
    }
}
