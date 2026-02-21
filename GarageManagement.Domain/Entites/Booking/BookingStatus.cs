namespace GarageManagement.Domain.Entites.Booking
{
    /// <summary>
    /// Lookup entity for booking status (DRAFT, SCHEDULED, CONFIRMED, etc.).
    /// Maps to [bkg].[BookingStatus].
    /// </summary>
    public class BookingStatus : BaseEntity
    {
        public string StatusName { get; set; } = string.Empty;
    }
}
