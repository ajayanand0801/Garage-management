using GarageManagement.Application.DTOs;
using GarageManagement.Domain.Entites.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.Interfaces
{
    public interface IVehicleRepository : IGenericRepository<Vehicle>
    {
        Task<VehicleDto?> GetVehicleWithDetailsAsync(long vehicleID);
        Task<IEnumerable<Vehicle>> GetAllVehiclesWithOwnersAsync();
        Task<long> GetMaxVehicleIdAsync();
        Task<VinSearchResponse?> GetVinSearchResponseAsync(string vin);
        Task<bool> VehicleExistsAsync(string engineNumber, string registrationNo, string vin, string chassisNumber);
        Task<bool> UpdateVehicleOwnersAsync(long vehicleId, List<VehicleOwnerDto> owners);
        IQueryable<Vehicle> GetAll();
    }

}
