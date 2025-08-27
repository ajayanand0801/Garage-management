using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Domain.Entites.Vehicles
{
    public class VehicleBrand : BaseEntity
    {
        public long Id { get; set; }
        public long BrandID { get; set; }  // UNIQUE in DB
        public string BrandName { get; set; }
        public ICollection<VehicleModel> VehicleModels { get; set; }
        public ICollection<Vehicle> Vehicles { get; set; }
    }
}
