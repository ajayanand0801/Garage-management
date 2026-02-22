using GarageManagement.Application.DTOs;
using GarageManagement.Domain.Entites.Booking;

namespace GarageManagement.Application.Interfaces
{
    /// <summary>
    /// Repository for [bkg].[BookingStatus] lookup. Used to validate status IDs for bookings and provide lookup data.
    /// </summary>
    public interface IBookingStatusRepository
    {
        /// <summary>
        /// Gets a booking status by Id if it exists and is active. Returns null if not found or inactive.
        /// </summary>
        Task<BookingStatus?> GetByIdAndActiveAsync(long id);
        Task<BookingStatus?> GetByIdAndActiveAsync(string name);

        /// <summary>
        /// Returns all active, non-deleted booking statuses as lookup DTOs (Id, Code, DisplayName).
        /// </summary>
        Task<IReadOnlyList<LookupDto>> GetLookupDtosAsync(CancellationToken cancellationToken = default);
    }
}
