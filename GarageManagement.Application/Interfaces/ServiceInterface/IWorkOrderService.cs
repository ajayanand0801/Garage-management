using GarageManagement.Application.DTOs;

namespace GarageManagement.Application.Interfaces.ServiceInterface
{
    public interface IWorkOrderService
    {
        Task<WorkOrderDto?> CreateWorkOrderAsync(CreateWorkOrderRequestDto request, CancellationToken cancellationToken = default);
        Task<WorkOrderDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    }
}
