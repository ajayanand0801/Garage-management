using GarageManagement.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.Interfaces
{
    public interface ICustomerRepository
    {
        IQueryable<Customer> GetAll();
        IQueryable<Customer> GetById(long id);
    }
}

