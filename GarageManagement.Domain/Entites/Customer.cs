using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarageManagement.Domain.Entites
{
    [Table("Customer", Schema = "dbo")]
    public class Customer : BaseEntity
    {
        [Required]
        public Guid CustomerGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long TenantID { get; set; }

        [Required]
        public long OrgID { get; set; }

        [Required]
        [StringLength(50)]
        public string CustomerType { get; set; } = null!; // "Individual" or "Company"

        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(20)]
        public string? MobileNo { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(500)]
        public string? Address2 { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? State { get; set; }

        [StringLength(10)]
        public string? CountryCode { get; set; }

        [StringLength(50)]
        public string? Country { get; set; }

        [StringLength(50)]
        public string? PostalCode { get; set; }

        [StringLength(200)]
        public string? CompanyName { get; set; }

        [StringLength(100)]
        public string? TradeLicenseNo { get; set; }

        [StringLength(50)]
        public string? TaxID { get; set; }

        [StringLength(100)]
        public string? RegistrationNumber { get; set; }

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [StringLength(20)]
        public string? ContactPhone { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public string? MetaData { get; set; }

        [StringLength(250)]
        public string? WebSite { get; set; }

        public int? ReferralSourceID { get; set; }

        public int? CountryID { get; set; }

        public int? StateID { get; set; }

        public int? CityID { get; set; }
    }
}

