using GarageManagement.Application.DTOs;
using GarageManagement.Application.Interfaces;
using GarageManagement.Domain.Entites.WorkOrder;
using GarageManagement.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace GarageManagement.Infrastructure.Repositories
{
    public class WorkOrderStatusRepository : IWorkOrderStatusRepository
    {
        private readonly RepairDbContext _context;

        public WorkOrderStatusRepository(RepairDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<LookupDto>> GetLookupDtosAsync(CancellationToken cancellationToken = default)
        {
            return await _context.WorkOrderStatuses
                .Where(s => s.IsActive && !s.IsDeleted)
                .OrderBy(s => s.Name)
                .Select(s => new LookupDto
                {
                    Id = s.Id,
                    Code = s.Code,
                    DisplayName = s.Name
                })
                .ToListAsync(cancellationToken);
        }
    }
}
