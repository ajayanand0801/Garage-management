using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.DTOs
{
    public class VehicleDomainDTO
    {
       
        public long? VehicleID { get; set; }

        [Required, MinLength(2)]
        public string Make { get; set; } = null!;

        [Required, MinLength(1)]
        public string Model { get; set; } = null!;

        [Required, Range(1900, 2100)]
        public int Year { get; set; }

        [Required, MinLength(5)]
        public string VIN { get; set; } = null!;



        [Required, MinLength(3)]
        public string LicensePlate { get; set; } = null!;
    }
}
