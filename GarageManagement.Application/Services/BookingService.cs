using ComponentManagement.PaginationUtility;
using GarageManagement.Application.DTOs;
using GarageManagement.Application.Interfaces;
using GarageManagement.Application.Interfaces.Mapper;
using GarageManagement.Application.Interfaces.ServiceInterface;
using GarageManagement.Domain.Entites.Booking;

namespace GarageManagement.Application.Services
{
    /// <summary>
    /// ObjectType value when the booking is linked to a ServiceRequest.
    /// </summary>
    public static class BookingObjectType
    {
        public const long ServiceRequest = 1;
    }

    public class BookingService : IBookingService
    {
        /// <summary>
        /// Generates a booking number in the format BK-{6-digit serial} using the booking Id.
        /// Used only when creating a booking (after insert); cannot be updated or modified by the application.
        /// </summary>
        private static string GenerateBookingNo(long bookingId)
        {
            const string prefix = "BK-";
            string serialPart = bookingId.ToString("D6"); // e.g., 000001, 000002
            return $"{prefix}{serialPart}";
        }

        private readonly IBookingRepository _bookingRepository;
        private readonly IMapperUtility _mapperUtility;
        private readonly IPaginationService<Booking> _paginationService;
        private readonly IBookingReferenceValidator _bookingReferenceValidator;

        public BookingService(
            IBookingRepository bookingRepository,
            IMapperUtility mapperUtility,
            IPaginationService<Booking> paginationService,
            IBookingReferenceValidator bookingReferenceValidator)
        {
            _bookingRepository = bookingRepository;
            _mapperUtility = mapperUtility;
            _paginationService = paginationService;
            _bookingReferenceValidator = bookingReferenceValidator;
        }

        public async Task<BookingDto?> GetByIdAsync(long id)
        {
            var entity = await _bookingRepository.GetByIdWithStatusAsync(id);
            if (entity == null)
                return null;
            var dto = _mapperUtility.Map<Booking, BookingDto>(entity);
            dto.StatusName = entity.StatusNavigation?.StatusName;
            return dto;
        }

        public async Task<PaginationResult<BookingDto>> GetPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken = default)
        {
            var query = _bookingRepository.GetQueryableForList();
            var paged = await _paginationService.PaginateAsync(query, request, cancellationToken);
            var items = paged.Items.Select(b =>
            {
                var dto = _mapperUtility.Map<Booking, BookingDto>(b);
                dto.StatusName = b.StatusNavigation?.StatusName;
                return dto;
            }).ToList();
            return new PaginationResult<BookingDto> { Items = items, TotalCount = paged.TotalCount };
        }

        public async Task<BookingDto?> CreateAsync(BookingDto dto, string? createdBy = null)
        {
            await _bookingReferenceValidator.ValidateAsync(dto);

            var entity = _mapperUtility.Map<BookingDto, Booking>(dto);
            entity.Id = 0;
            entity.BookingGuid = Guid.NewGuid();
            entity.ServiceRequestID = dto.ServiceRequestId;
            entity.ObjectType = BookingObjectType.ServiceRequest;
            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedBy = createdBy ?? dto.CreatedBy ?? "System";
            entity.IsActive = true;
            entity.IsDeleted = false;
            entity.DurationType = dto.DurationType ?? "hourly";

            var added = await _bookingRepository.AddAsync(entity);
            if (!added)
                return null;

            // Generate BookingNo from assigned Id (server-side only; never accepted from client or updated)
            entity.BookingNo = GenerateBookingNo(entity.Id);
            await _bookingRepository.UpdateAsync(entity);

            dto.Id = entity.Id;
            dto.BookingNo = entity.BookingNo;
            dto.BookingGuid = entity.BookingGuid;
            dto.CreatedAt = entity.CreatedAt;
            dto.CreatedBy = entity.CreatedBy;
            return dto;
        }

        public async Task<bool> UpdateAsync(long id, BookingDto dto, string? modifiedBy = null)
        {
            await _bookingReferenceValidator.ValidateAsync(dto);

            var existing = await _bookingRepository.GetByIdWithStatusAsync(id);
            if (existing == null)
                return false;

            _mapperUtility.Map(dto, existing);
            existing.Id = id;
            existing.BookingGuid = existing.BookingGuid;
            existing.BookingNo = existing.BookingNo; // Immutable: never updated or modified from application
            existing.ServiceRequestID = dto.ServiceRequestId;
            existing.ObjectType = BookingObjectType.ServiceRequest;
            existing.ModifiedAt = DateTime.UtcNow;
            existing.ModifiedBy = modifiedBy ?? dto.ModifiedBy ?? "System";
            existing.DurationType = dto.DurationType ?? existing.DurationType ?? "hourly";

            return await _bookingRepository.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            return await _bookingRepository.SoftDelete(id);
        }
    }
}
