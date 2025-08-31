using GarageManagement.Application.DTOs;
using GarageManagement.Domain.Entites.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.Interfaces.ServiceInterface
{
    public  interface IVehicleLookupService
    {
        Task<IEnumerable<VehicleLookupDTO>> GetAllVehicleLookupsAsync();
        Task<VehicleLookupDTO?> GetVehicleLookupByIdAsync(int id);
        Task<bool> CreateVehicleLookupAsync(VehicleLookupDTO vehicleLookup);
        Task<bool> UpdateVehicleLookupAsync(int id, VehicleLookupDTO updated);
        Task<bool> DeleteVehicleLookupAsync(int id);
        bool ValidateVehicleLookup(VehicleLookupDTO vehicleLookupDto, out List<string> errors);
        Task<bool> CreateVehicleLookup(VehicleLookupDTO vehicleLookupRequest);
        Task<IEnumerable<VehicleLookupDTO>> GetVehicleLookupsByTypeAsync(string lookupType);
    }
}
