using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Domain.Entites.Vehicles
{
    public class VehicleModelYear : BaseEntity
    {
        public long ModelYearID { get; set; }
        public long ModelID { get; set; }
        public int ModelYear { get; set; }
        public string? Value { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public VehicleModel Model { get; set; } = null!;
        public ICollection<VehicleVariant> Variants { get; set; } = new List<VehicleVariant>();
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }

}
