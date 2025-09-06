using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.DTOs
{
    public class CustomerDto
    {
        public long? CustomerID { get; set; }

      
        public string FirstName { get; set; } = null!;

       
        public string LastName { get; set; } = null!;

     
        public string Email { get; set; } = null!;

       
        public string? Phone { get; set; } = null!;
        public string MobilePhone { get; set; } = null!;


        public string Address { get; set; } = null!;

      
        public string City { get; set; } = null!;

      
        public string State { get; set; } = null!;

      
        public string? PostalCode { get; set; } = null!;

        
        public string? Country { get; set; } = null!;
    }

}
