using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.DTOs
{
    public class DomainDataDto
    {
       
        public VehicleDomainDTO Vehicle { get; set; } = null!;

      
        public QuotationDTO? Quotation { get; set; } = null!;

        
    }
}
