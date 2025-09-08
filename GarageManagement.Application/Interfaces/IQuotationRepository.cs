using GarageManagement.Domain.Entites.Quotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.Interfaces
{
    public interface IQuotationRepository
    {


        // Read
        IQueryable<Quotation> GetById(long id);

       
        IQueryable<Quotation> GetAll();


    }
}
