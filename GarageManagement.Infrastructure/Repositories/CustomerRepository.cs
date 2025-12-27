using GarageManagement.Application.Interfaces;
using GarageManagement.Domain.Entites;
using GarageManagement.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly RepairDbContext _context;

        public CustomerRepository(RepairDbContext context)
        {
            _context = context;
        }

        public IQueryable<Customer> GetAll()
        {
            return _context.Customers
                           .Where(c => !c.IsDeleted && c.IsActive);
        }

        public IQueryable<Customer> GetById(long id)
        {
            return _context.Customers
                           .Where(c => c.Id == id && !c.IsDeleted && c.IsActive);
        }
    }
}

