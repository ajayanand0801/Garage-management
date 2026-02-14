using GarageManagement.Application.Interfaces;
using GarageManagement.Infrastructure.DbContext;
using GarageManagement.Domain.Entites.WorkOrder;
using Microsoft.EntityFrameworkCore;

namespace GarageManagement.Infrastructure.Repositories
{
    public class WorkOrderRepository : IWorkOrderRepository
    {
        private readonly RepairDbContext _context;

        public WorkOrderRepository(RepairDbContext context)
        {
            _context = context;
        }

        public async Task<WorkOrder?> GetByIdAsync(long id)
        {
            return await _context.WorkOrders
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<bool> AddAsync(WorkOrder entity)
        {
            try
            {
                await _context.WorkOrders.AddAsync(entity);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }
    }
}
