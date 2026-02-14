using GarageManagement.Application.Interfaces;
using GarageManagement.Domain.Entites.Quotation;
using GarageManagement.Domain.Entites.Vehicles;
using GarageManagement.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Infrastructure.Repositories
{
    public class QuotationRepository : IQuotationRepository
    {
        private readonly RepairDbContext _context;

        public QuotationRepository(RepairDbContext context)
        {
            _context = context;
        }

        public IQueryable<Quotation> GetAll()
        {
            return _context.Quotations
                           .Include(q => q.QuotationItems)
                           .Where(q => !q.IsDeleted && q.IsActive);
        }


        
        public IQueryable<Quotation> GetById(long id)
        {
            return _context.Quotations
                           .Include(q => q.QuotationItems.Where(item => item.IsActive))
                           .Where(q => q.Id == id && !q.IsDeleted && q.IsActive);
        }

        public IQueryable<QuotationItem> GetQuotationByQuoteId(long quoteId)
        {
            return _context.QuotationItem
                           .Where(q => q.QuotationID == quoteId && !q.IsDeleted && q.IsActive);
                           
        }

        public async Task<long?> GetMaxQuotationIdAsync()
        {
            return await _context.Quotations
                .Where(q => !q.IsDeleted && q.IsActive && q.QuotationId != null)
                .MaxAsync(q => q.QuotationId);
        }
    }
}
