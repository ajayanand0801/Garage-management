using ComponentManagement.PaginationUtility;
using GarageManagement.Application.DTOs;

namespace GarageManagement.Application.Interfaces.ServiceInterface
{
    public interface IWorkOrderService
    {
        Task<WorkOrderDto?> CreateWorkOrderAsync(CreateWorkOrderRequestDto request, CancellationToken cancellationToken = default);
        Task<WorkOrderDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(long id, WorkOrderDto dto, string? modifiedBy = null);
        Task<bool> DeleteAsync(long id);
        Task<PaginationResult<WorkOrderDto>> GetPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken = default);
    }
}
