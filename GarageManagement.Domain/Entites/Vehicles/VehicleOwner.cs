using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Domain.Entites.Vehicles
{
    public class VehicleOwner : BaseEntity
    {
       
     
        public long VehicleID { get; set; }
        public string? VehicleVIN { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
        public string? ContactNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public DateTime OwnershipStartDate { get; set; }
        public DateTime? OwnershipEndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public long OwnerID { get; set; }
        public int Type { get; set; }// 1 - individual,2 Company,3.Fleet,
       public string? TradeLicenseNo { get; set; }


        public Vehicle Vehicle { get; set; } = null!;
    }

}
