using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Domain.Entites.Vehicles
{
    public class VehicleLookup:BaseEntity
    {
        [Key]
        public int LookupID { get; set; }

        [Required]
        [StringLength(50)]
        public string LookupType { get; set; }

        [Required]
        [StringLength(100)]
        public string LookupValue { get; set; }
        [NotMapped]
        public new long Id { get; set; }
    }
}
