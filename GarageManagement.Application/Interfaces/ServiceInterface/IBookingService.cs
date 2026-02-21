using ComponentManagement.PaginationUtility;
using GarageManagement.Application.DTOs;

namespace GarageManagement.Application.Interfaces.ServiceInterface
{
    public interface IBookingService
    {
        Task<BookingDto?> GetByIdAsync(long id);
        Task<PaginationResult<BookingDto>> GetPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken = default);
        Task<BookingDto?> CreateAsync(BookingDto dto, string? createdBy = null);
        Task<bool> UpdateAsync(long id, BookingDto dto, string? modifiedBy = null);
        Task<bool> DeleteAsync(long id);
    }
}
