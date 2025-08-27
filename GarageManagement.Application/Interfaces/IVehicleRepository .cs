﻿using GarageManagement.Application.DTOs;
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
    }

}
