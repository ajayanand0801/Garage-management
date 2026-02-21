using GarageManagement.Application.DTOs;
using GarageManagement.Application.Enums;
using GarageManagement.Application.Interfaces;
using GarageManagement.Application.Interfaces.ServiceInterface;

namespace GarageManagement.Application.Services
{
    public class LookupService : ILookupService
    {
        private readonly IGarageServiceRepository _garageServiceRepository;

        public LookupService(IGarageServiceRepository garageServiceRepository)
        {
            _garageServiceRepository = garageServiceRepository;
        }

        public async Task<IReadOnlyList<LookupDto>> GetLookupAsync(LookupType type, CancellationToken cancellationToken = default)
        {
            return type switch
            {
                LookupType.GarageService => await _garageServiceRepository.GetLookupDtosAsync(cancellationToken),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported lookup type.")
            };
        }
    }
}
