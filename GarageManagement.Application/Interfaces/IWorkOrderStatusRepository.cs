using GarageManagement.Application.DTOs;
using GarageManagement.Domain.Entites.WorkOrder;

namespace GarageManagement.Application.Interfaces
{
    /// <summary>
    /// Repository for [rpa].[WorkOrderStatus] lookup. Used to provide work order status lookup data.
    /// </summary>
    public interface IWorkOrderStatusRepository
    {
        /// <summary>
        /// Returns all active, non-deleted work order statuses as lookup DTOs (Id, Code, DisplayName).
        /// </summary>
        Task<IReadOnlyList<LookupDto>> GetLookupDtosAsync(CancellationToken cancellationToken = default);
    }
}
