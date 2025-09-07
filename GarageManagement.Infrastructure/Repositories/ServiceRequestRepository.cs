using GarageManagement.Application.Interfaces;
using GarageManagement.Domain.Entites.Request;
using GarageManagement.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Infrastructure.Repositories
{
    public class ServiceRequestRepository : GenericRepository<ServiceRequest>, IServiceRequestRepository
    {
        private readonly RepairDbContext _context;

        public ServiceRequestRepository(RepairDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceRequest>> GetAllActiveRequestsAsync()
        {
            return await _context.ServiceRequests
                .Where(sr => sr.IsActive && !sr.IsDeleted)
                .ToListAsync();
        }

        public async Task<ServiceRequest?> GetRequestWithDetailsAsync(long requestId)
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Documents)
                .Include(sr => sr.MetadataEntries)
                .FirstOrDefaultAsync(sr => sr.Id == requestId);
        }

        public async Task<string?> GetRequestStatusAsync(long requestId)
        {
            return await _context.ServiceRequests
                .Where(sr => sr.Id == requestId)
                .Select(sr => sr.Status)
                .FirstOrDefaultAsync();
        }

        public async Task<long> GetMaxServiceRequestIdAsync()
        {
            var maxId = await _context.ServiceRequests.MaxAsync(sr => (long?)sr.Id);
            return maxId ?? 10000;
        }
    }
}
