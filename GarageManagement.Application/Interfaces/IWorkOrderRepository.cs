using GarageManagement.Domain.Entites.WorkOrder;

namespace GarageManagement.Application.Interfaces
{
    public interface IWorkOrderRepository
    {
        Task<WorkOrder?> GetByIdAsync(long id);
        Task<bool> AddAsync(WorkOrder entity);
    }
}
