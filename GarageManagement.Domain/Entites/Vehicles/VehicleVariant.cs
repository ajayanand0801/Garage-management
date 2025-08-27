using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Domain.Entites.Vehicles
{
    public class VehicleVariant : BaseEntity
    {
        public long VariantID { get; set; }
        public long ModelYearID { get; set; }
        public string VariantName { get; set; } = null!;
        public string? EngineType { get; set; }
        public decimal? EngineCapacity { get; set; }
        public string? Transmission { get; set; }
        public string? FuelType { get; set; }
        public int? SeatingCapacity { get; set; }
        public string? Features { get; set; }
        public decimal? Price { get; set; }
        public long NoOfDoors { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public VehicleModelYear ModelYear { get; set; } = null!;
        //public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }

}
