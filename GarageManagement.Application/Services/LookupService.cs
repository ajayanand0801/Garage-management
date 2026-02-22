using GarageManagement.Application.DTOs;
using GarageManagement.Application.Enums;
using GarageManagement.Application.Interfaces;
using GarageManagement.Application.Interfaces.ServiceInterface;

namespace GarageManagement.Application.Services
{
    public class LookupService : ILookupService
    {
        private readonly IGarageServiceRepository _garageServiceRepository;
        private readonly IBookingStatusRepository _bookingStatusRepository;
        private readonly IServiceCategoryRepository _serviceCategoryRepository;

        public LookupService(
            IGarageServiceRepository garageServiceRepository,
            IBookingStatusRepository bookingStatusRepository,
            IServiceCategoryRepository serviceCategoryRepository)
        {
            _garageServiceRepository = garageServiceRepository;
            _bookingStatusRepository = bookingStatusRepository;
            _serviceCategoryRepository = serviceCategoryRepository;
        }

        public async Task<IReadOnlyList<LookupDto>> GetLookupAsync(LookupType type, CancellationToken cancellationToken = default)
        {
            return type switch
            {
                LookupType.GarageService => await _garageServiceRepository.GetLookupDtosAsync(cancellationToken),
                LookupType.BookingStatus => await _bookingStatusRepository.GetLookupDtosAsync(cancellationToken),
                LookupType.ServiceCategory => await _serviceCategoryRepository.GetLookupDtosAsync(cancellationToken),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported lookup type.")
            };
        }

        public async Task<IReadOnlyList<LookupDto>> GetLookupByTypeNameAsync(string typeName, CancellationToken cancellationToken = default)
        {
            var resolved = ResolveLookupTypeFromName(typeName);
            return await GetLookupAsync(resolved, cancellationToken);
        }

        /// <summary>
        /// Maps type name (string) to LookupType. "Type" maps to ServiceCategory. Enum names are case-insensitive.
        /// </summary>
        private static LookupType ResolveLookupTypeFromName(string? typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentOutOfRangeException(nameof(typeName), typeName, "Lookup type name is required.");

            var name = typeName.Trim();
            if (string.Equals(name, "BookingType", StringComparison.OrdinalIgnoreCase))
                return LookupType.ServiceCategory;

            if (Enum.TryParse<LookupType>(name, ignoreCase: true, out var type))
                return type;

            throw new ArgumentOutOfRangeException(nameof(typeName), typeName, $"Unsupported lookup type. Use: GarageService, BookingStatus,  or BookingType.");
        }
    }
}
