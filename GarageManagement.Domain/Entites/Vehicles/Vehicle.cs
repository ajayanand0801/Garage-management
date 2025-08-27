﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Domain.Entites.Vehicles
{
    [Table("vehicle", Schema = "vhc")]

    public class Vehicle : BaseEntity
    {
       

        [Required]
        public long VehicleID { get; set; }

        [Required]
        [StringLength(17)]
        public string VIN { get; set; }

        [Required]
        public long BrandID { get; set; }

        [Required]
        public long ModelID { get; set; }

        [Required]
        public long ModelYearID { get; set; }

        [StringLength(50)]
        public string? Color { get; set; }

        [StringLength(20)]
        public string? RegistrationNumber { get; set; }

        [StringLength(50)]
        public string? EngineNumber { get; set; }

        [StringLength(50)]
        public string? ChassisNumber { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        [StringLength(100)]
        public string? ModifiedBy { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
        // Navigation properties
        public VehicleBrand? Brand { get; set; }
        public VehicleModel? Model { get; set; }
        public VehicleModelYear? ModelYear { get; set; }
        // Variant navigation property removed
        // public VehicleVariant Variant { get; set; } // Removed

        public ICollection<VehicleOwner>? Owners { get; set; }


    }

}
