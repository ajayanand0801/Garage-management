using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.DTOs
{
    public class CrmCustomerDTO
    {
        public long? Id { get; set; }
        public Guid? CustomerGuid { get; set; }

        public long? TenantId { get; set; }
        public long? OrgId { get; set; }

        public string CustomerType { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? Phone { get; set; }
        public string? MobileNo { get; set; }
        public string? Email { get; set; }

        public string? Address { get; set; }
        public string? Address2 { get; set; }

        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? CountryCode { get; set; }
        public string? PostalCode { get; set; }

        public int? CityId { get; set; }
        public int? StateId { get; set; }
        public int? CountryId { get; set; }

        public string? CompanyName { get; set; }
        public string? TradeLicenseNo { get; set; }
        public string? TaxId { get; set; }
        public string? RegistrationNumber { get; set; }

        public string? ContactPerson { get; set; }
        public string? ContactPhone { get; set; }

        public string? WebSite { get; set; }
        public int? ReferralSourceId { get; set; }

        public string? MetaData { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
