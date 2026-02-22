using ComponentManagement.PaginationUtility;
using GarageManagement.Application.DTOs;
using GarageManagement.Application.Interfaces;
using GarageManagement.Application.Interfaces.Mapper;
using GarageManagement.Application.Interfaces.ServiceInterface;
using GarageManagement.Domain.Entites;
using GarageManagement.Domain.Entites.Booking;
using GarageManagement.Domain.Entites.Service;
using GarageManagement.Domain.Entites.Vehicles;
using System.ComponentModel.DataAnnotations;

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
        private readonly IGenericRepository<Customer> _customerRepository;
        private readonly IGenericRepository<Vehicle> _vehicleRepository;
        private readonly IGarageServiceRepository _garageServiceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookingStatusRepository _bookingStatusRepository;
        private readonly IServiceCategoryRepository _serviceCategoryRepository;

        public BookingService(
            IBookingRepository bookingRepository,
            IMapperUtility mapperUtility,
            IPaginationService<Booking> paginationService,
            IBookingReferenceValidator bookingReferenceValidator,
            IGenericRepository<Customer> customerRepository,
            IGenericRepository<Vehicle> vehicleRepository,
            IGarageServiceRepository garageServiceRepository,
            IUnitOfWork unitOfWork,
            IBookingStatusRepository bookingStatusRepository,
            IServiceCategoryRepository serviceCategoryRepository)
        {
            _bookingRepository = bookingRepository;
            _mapperUtility = mapperUtility;
            _paginationService = paginationService;
            _bookingReferenceValidator = bookingReferenceValidator;
            _customerRepository = customerRepository;
            _vehicleRepository = vehicleRepository;
            _garageServiceRepository = garageServiceRepository;
            _unitOfWork = unitOfWork;
            _bookingStatusRepository = bookingStatusRepository;
            _serviceCategoryRepository = serviceCategoryRepository;
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

        public async Task<BookingDetailsDto?> GetByIdWithDetailsAsync(long id)
        {
            var entity = await _bookingRepository.GetByIdWithStatusAsync(id);
            if (entity == null)
                return null;

            var dto = _mapperUtility.Map<Booking, BookingDetailsDto>(entity);

            // Sequential awaits: all repos share the same scoped DbContext; EF Core does not support concurrent operations on one context.
            var customer = await _customerRepository.GetByIdAsync(entity.CustomerID, null);
            if (customer != null)
                dto.CustomerDetails = _mapperUtility.Map<Customer, BookingCustomerDetailDto>(customer);

            if (entity.VehicleID.HasValue && entity.VehicleID.Value > 0)
            {
                var vehicle = await _vehicleRepository.GetByVehicleIdAsync(entity.VehicleID.Value, null);
                if (vehicle != null)
                    dto.VehicleDetails = _mapperUtility.Map<Vehicle, BookingVehicleDetailDto>(vehicle);
            }

            var garageService = await _garageServiceRepository.GetByIdAsync(entity.ServiceID, null);
            if (garageService != null)
                dto.ServiceDetails = _mapperUtility.Map<GarageService, BookingServiceDetailDto>(garageService);

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
            var serviceCategory = await _serviceCategoryRepository.GetByCategoryNameByCodeAsync(dto.Type.Trim());
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
            // Generate BookingNo from assigned Id (server-side only; never accepted from client or updated)
            entity.BookingNo = GenerateBookingNo(entity.Id);
            entity.Type = serviceCategory!=null ? serviceCategory.CategoryName:string.Empty;
            await _unitOfWork.Booking.AddTransactionAsync(entity);
            await _unitOfWork.CommitAsync();

          
            return _mapperUtility.Map<Booking, BookingDto>(entity);
        }

        /// <summary>
        /// Adds a booking to the current unit-of-work transaction. Does not commit; caller commits. Used when creating a booking as part of service request create (same transaction).
        /// BookingNo is generated from max Id in booking table + 1, format BK-000001, BK-000002, etc.
        /// </summary>
        public async Task AddBookingTransactionAsync(BookingDto dto, string? createdBy = null)
        {       

            var entity = _mapperUtility.Map<BookingDto, Booking>(dto);
            entity.Id = 0;
            entity.BookingGuid = Guid.NewGuid();
            entity.ServiceRequestID = dto.ServiceRequestId;
            entity.ObjectType = BookingObjectType.ServiceRequest;
            entity.StatusID = dto.StatusID > 0 ? dto.StatusID : 1;
            entity.StartDate = dto.StartDate == default ? DateTime.UtcNow : dto.StartDate;
            entity.DurationType = dto.DurationType ?? "hourly";
            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedBy = createdBy ?? dto.CreatedBy ?? "System";
            entity.IsActive = true;
            entity.IsDeleted = false;
            await _bookingReferenceValidator.ValidateAsync(dto);
            var serviceCategory = await _serviceCategoryRepository.GetByCategoryNameByCodeAsync(dto.Type.Trim());
            entity.Type=serviceCategory!=null? serviceCategory.CategoryName: null;
            long maxId = await _bookingRepository.GetMaxIdAsync();
            entity.BookingNo = $"BK-{(maxId + 1):D6}";

            await _unitOfWork.Booking.AddTransactionAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(long id, BookingDto dto, string? modifiedBy = null)
        {
            await _bookingReferenceValidator.ValidateAsync(dto);
            var serviceCategory = await _serviceCategoryRepository.GetByCategoryNameByCodeAsync(dto.Type.Trim());

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
            existing.Type=serviceCategory!=null ? serviceCategory.CategoryName: null;

            return await _bookingRepository.UpdateAsync(existing);
        }

        public async Task<BookingDto?> PatchAsync(long id, BookingPatchDto dto, string? modifiedBy = null)
        {
            if (dto == null)
                throw new ValidationException("Patch payload cannot be null.");

            var existing = await _bookingRepository.GetByIdWithStatusAsync(id);
            if (existing == null)
                return null;

            var now = DateTime.UtcNow;
            var today = now.Date;
            bool anyPatch = false;

            //if (dto.StatusID.HasValue)
            //{
            //    var status = await _bookingStatusRepository.GetByIdAndActiveAsync(dto.StatusID.Value);
            //    if (status == null)
            //        throw new ValidationException($"Invalid booking status. StatusID {dto.StatusID.Value} does not exist or is not active in [bkg].[BookingStatus].");
            //    existing.StatusID = dto.StatusID.Value;
            //    existing.Status = status.StatusName;
            //    anyPatch = true;
            //}

            if (dto.Status != null)
            {
                var status = await _bookingStatusRepository.GetByIdAndActiveAsync(dto.Status);
                if (status == null)
                    throw new ValidationException($"Invalid booking status.  {dto.Status}.");
                existing.StatusID = status.Id;
                existing.Status = status.StatusName;
                anyPatch = true;
            }

            if (dto.TypeCode != null)
            {
                var serviceCategory = await _serviceCategoryRepository.GetByCategoryNameByCodeAsync(dto.TypeCode.Trim());
                if (serviceCategory == null)
                    throw new ValidationException($"Invalid booking TypeCode. '{dto.TypeCode}'.");
                existing.Type = serviceCategory.CategoryName.Trim();
                anyPatch = true;
            }

            if (dto.StartDate.HasValue)
            {
                if (dto.StartDate.Value.Date < today)
                    throw new ValidationException("StartDate cannot be a past date.");
                existing.StartDate = dto.StartDate.Value;
                anyPatch = true;
            }

            if (dto.EndDate.HasValue)
            {
                if (dto.EndDate.Value < now )
                    throw new ValidationException("EndDate cannot be in the past (must be >= current date and time).");
                existing.EndDate = dto.EndDate.Value;
                anyPatch = true;
            }

            if (dto.DurationType != null)
            {
                existing.DurationType = dto.DurationType;
                anyPatch = true;
            }

            if (dto.Notes != null)
            {
                existing.Notes = dto.Notes;
                anyPatch = true;
            }

            if (!anyPatch)
                return await GetByIdAsync(id);

            existing.ModifiedAt = now;
            existing.ModifiedBy = modifiedBy ?? "System";

            // Save via UnitOfWork; EF Core updates only modified properties on the tracked entity (partial UPDATE)
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            return await _bookingRepository.SoftDelete(id);
        }
    }
}
