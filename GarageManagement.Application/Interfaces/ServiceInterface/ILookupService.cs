using GarageManagement.Application.DTOs;
using GarageManagement.Application.Enums;

namespace GarageManagement.Application.Interfaces.ServiceInterface
{
    public interface ILookupService
    {
        /// <summary>
        /// Returns lookup items for the given type. Standard payload: Id, Code, DisplayName.
        /// </summary>
        Task<IReadOnlyList<LookupDto>> GetLookupAsync(LookupType type, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns lookup items by type name (string). Accepts: "GarageService", "BookingStatus", "ServiceCategory", or "Type" (maps to ServiceCategory).
        /// Case-insensitive. Standard payload: Id, Code, DisplayName.
        /// </summary>
        Task<IReadOnlyList<LookupDto>> GetLookupByTypeNameAsync(string typeName, CancellationToken cancellationToken = default);
    }
}
