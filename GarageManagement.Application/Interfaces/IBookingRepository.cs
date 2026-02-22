using GarageManagement.Domain.Entites.Booking;

namespace GarageManagement.Application.Interfaces
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        /// <summary>
        /// Returns queryable for paginated list with status included, excluding soft-deleted.
        /// </summary>
        IQueryable<Booking> GetQueryableForList();

        /// <summary>
        /// Gets booking by id with status, for update/detail.
        /// </summary>
        Task<Booking?> GetByIdWithStatusAsync(long id);

        /// <summary>
        /// Gets the current max Id from the booking table (excluding soft-deleted). Used to generate next BookingNo.
        /// </summary>
        Task<long> GetMaxIdAsync();
    }
}
