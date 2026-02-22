using ComponentManagement.PaginationUtility;
using GarageManagement.Application.DTOs;

namespace GarageManagement.Application.Interfaces.ServiceInterface
{
    public interface IBookingService
    {
        Task<BookingDto?> GetByIdAsync(long id);
        /// <summary>Gets booking by id with optional customer, vehicle, and service details (nullable when not found).</summary>
        Task<BookingDetailsDto?> GetByIdWithDetailsAsync(long id);
        Task<PaginationResult<BookingDto>> GetPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken = default);
        Task<BookingDto?> CreateAsync(BookingDto dto, string? createdBy = null);
        /// <summary>Adds a booking to the current unit-of-work transaction (no commit). Used when creating a booking as part of another operation (e.g. service request create). Caller must set dto.ServiceRequestId and ensure tenant/org/domain and IDs are set.</summary>
        Task AddBookingTransactionAsync(BookingDto dto, string? createdBy = null);
        Task<bool> UpdateAsync(long id, BookingDto dto, string? modifiedBy = null);
        /// <summary>Partial update by bookingId: StatusID, Status, Type, StartDate, EndDate, DurationType, Notes. Validates status from BookingStatus and dates not in past.</summary>
        Task<BookingDto?> PatchAsync(long id, BookingPatchDto dto, string? modifiedBy = null);
        Task<bool> DeleteAsync(long id);
    }
}
