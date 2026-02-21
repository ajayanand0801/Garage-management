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
    }
}
