using GarageManagement.Application.DTOs;
using GarageManagement.Application.Interfaces;
using GarageManagement.Domain.Entites.Service;
using GarageManagement.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace GarageManagement.Infrastructure.Repositories
{
    public class GarageServiceRepository : GenericRepository<GarageService>, IGarageServiceRepository
    {
        private readonly RepairDbContext _context;

        public GarageServiceRepository(RepairDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<LookupDto>> GetLookupDtosAsync(CancellationToken cancellationToken = default)
        {
            return await _context.GarageServices
                .Where(gs => gs.IsActive && !gs.IsDeleted)
                .Select(gs => new LookupDto
                {
                    Id = gs.Id,
                    Code = gs.Code,
                    DisplayName = gs.ServiceName
                })
                .ToListAsync(cancellationToken);
        }
    }
}
