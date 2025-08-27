using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Domain.Entites.Vehicles
{
    public class VehicleModel : BaseEntity
    {
        public long ModelID { get; set; }
        public long BrandID { get; set; }
        public string ModelName { get; set; } = null!;
        public int? BodyTypeID { get; set; }
        public string? BodyType { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public VehicleBrand Brand { get; set; } = null!;
        public ICollection<VehicleModelYear> ModelYears { get; set; } = new List<VehicleModelYear>();
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }

}
