using ComponentManagement.PaginationUtility;
using GarageManagement.Application.DTOs;
using GarageManagement.Domain.Entites.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.Interfaces.ServiceInterface
{
    public interface IVehicleService
    {
        Task<IEnumerable<Vehicle>> GetAllVehiclesAsync();
        Task<VehicleDto?> GetVehicleByIdAsync(long id);
        Task<bool> CreateVehicleAsync(Vehicle vehicle);
        Task<bool> CreateVehicle(VehicleDto vehicleRequest);
        Task<bool> UpdateVehicleAsync(long id, Vehicle vehicle);
        Task<bool> DeleteVehicleAsync(long id);
        Task<VinSearchResponse> GetVehicleBYVin(string  vin);
        Task<bool> UpdateVehicleOwnersAsync(long vehicleId, List<VehicleOwnerDto> owners);
        Task<PaginationResult<VehicleDto>> GetAllVehiclesAsync(PaginationRequest request, CancellationToken cancellationToken);
    }

}
