using GarageManagement.Application.DTOs;
using GarageManagement.Domain.Entites.Service;

namespace GarageManagement.Application.Interfaces
{
    /// <summary>
    /// Repository for [rpa].[ServiceCategory] lookup. Used to validate booking Type by category name and provide lookup data.
    /// </summary>
    public interface IServiceCategoryRepository
    {
        /// <summary>
        /// Gets a service category by CategoryName (case-insensitive) if it exists and is active. Returns null if not found or inactive.
        /// </summary>
        Task<ServiceCategory?> GetByCategoryNameAndActiveAsync(string categoryName);
        Task<ServiceCategory?> GetByCategoryNameByCodeAsync(string code);

        /// <summary>
        /// Returns all active, non-deleted service categories as lookup DTOs (Id, Code, DisplayName).
        /// </summary>
        Task<IReadOnlyList<LookupDto>> GetLookupDtosAsync(CancellationToken cancellationToken = default);
    }
}
