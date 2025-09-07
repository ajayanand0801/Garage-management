using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Domain.Entites.Request
{
    public class ServiceRequestCustomerMetaData:BaseEntity
    {
      
        public Guid? CustomerMetaGuid { get; set; }
        public long? CustomerID { get; set; }
        public long TenantID { get; set; }
        public long ?OrgID { get; set; }
        public string CustomerType { get; set; } = null!; // "Individual" or "Company"
        [Required]
        public long RequestID { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? MobileNo { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? CountryCode { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }

        public string? CompanyName { get; set; }
        public string? TradeLicenseNo { get; set; }
        public string? TaxID { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPhone { get; set; }

        public ServiceRequest ServiceRequest { get; set; }


    }

}
