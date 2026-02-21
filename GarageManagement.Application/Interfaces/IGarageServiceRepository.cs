using GarageManagement.Application.DTOs;
using GarageManagement.Domain.Entites.Service;

namespace GarageManagement.Application.Interfaces
{
    public interface IGarageServiceRepository : IGenericRepository<GarageService>
    {
        /// <summary>
        /// Returns all active, non-deleted garage services as lookup DTOs.
        /// </summary>
        Task<IReadOnlyList<LookupDto>> GetLookupDtosAsync(CancellationToken cancellationToken = default);
    }
}
