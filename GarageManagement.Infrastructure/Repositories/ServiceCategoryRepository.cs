using GarageManagement.Application.DTOs;
using GarageManagement.Application.Interfaces;
using GarageManagement.Domain.Entites.Service;
using GarageManagement.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace GarageManagement.Infrastructure.Repositories
{
    public class ServiceCategoryRepository : IServiceCategoryRepository
    {
        private readonly RepairDbContext _context;

        public ServiceCategoryRepository(RepairDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceCategory?> GetByCategoryNameAndActiveAsync(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                return null;

            return await _context.ServiceCategories
                .Where(c => c.CategoryName.ToLower() == categoryName.Trim().ToLower() && c.IsActive && !c.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<ServiceCategory?> GetByCategoryNameByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return null;

            return await _context.ServiceCategories
                .Where(c => c.Code.ToLower() == code.Trim().ToLower() && c.IsActive && !c.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<LookupDto>> GetLookupDtosAsync(CancellationToken cancellationToken = default)
        {
            return await _context.ServiceCategories
                .Where(c => c.IsActive && !c.IsDeleted)
                .OrderBy(c => c.CategoryName)
                .Select(c => new LookupDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    DisplayName = c.CategoryName
                })
                .ToListAsync(cancellationToken);
        }
    }
}
