using GarageManagement.Application.Interfaces;
using GarageManagement.Domain.Entites.WorkOrder;
using GarageManagement.Infrastructure.DbContext;
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
                .Where(e => e.Id == id && !e.IsDeleted)
                .FirstOrDefaultAsync();
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

        public async Task<bool> UpdateAsync(WorkOrder entity)
        {
            try
            {
                _context.WorkOrders.Update(entity);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool> SoftDeleteAsync(long id)
        {
            var entity = await _context.WorkOrders
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
            if (entity == null)
                return false;
            entity.IsDeleted = true;
            entity.IsActive = false;
            entity.ModifiedAt = DateTime.UtcNow;
            entity.ModifiedBy = "System";
            _context.WorkOrders.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public IQueryable<WorkOrder> GetQueryableForList()
        {
            return _context.WorkOrders
                .Where(w => !w.IsDeleted)
                .OrderByDescending(w => w.CreatedAt);
        }
    }
}
