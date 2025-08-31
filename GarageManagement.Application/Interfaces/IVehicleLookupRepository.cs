using GarageManagement.Application.DTOs;
using GarageManagement.Domain.Entites.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.Interfaces
{
    public interface IVehicleLookupRepository : IGenericRepository<VehicleLookup>
    {
        Task<IEnumerable<VehicleLookupDTO>> GetAllVehicleLookupsAsync();
        Task<VehicleLookupDTO?> GetVehicleLookupWithDetailsAsync(int id);
        Task<IEnumerable<VehicleLookupDTO>> GetVehicleLookupsByTypeAsync(string lookupType);
    }

}
