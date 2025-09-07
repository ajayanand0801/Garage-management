using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Domain.Entites.Request
{
    public class ServiceRequestVehicleMetaData:BaseEntity
    {



        public long? VehicleId { get; set; }

        [Required]
        public long TenantID { get; set; }

        public long? OrgID { get; set; }

        [Required]
        public long RequestID { get; set; }

        [Required]
        [StringLength(100)]
        public string Make { get; set; }

        [Required]
        [StringLength(100)]
        public string Model { get; set; }

        [Required]
        [Range(1900, 2100)]
        public int Year { get; set; }

        [Required]
        [StringLength(100)]
        public string VIN { get; set; }

        [StringLength(50)]
        public string? LicensePlate { get; set; }

        public long? Mileage { get; set; }

        public long? OwnerID { get; set; }

        [Required]
        [StringLength(150)]
        public string OwnerName { get; set; }

        [StringLength(250)]
        public string? OwnerType { get; set; }

        [StringLength(20)]
        public string    ContactNumber { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        public DateTime? RegDate { get; set; }
        public DateTime? ManufactureDate { get; set; }

        public string? MetaData { get; set; }

        public ServiceRequest ServiceRequest { get; set; }
    }
}
