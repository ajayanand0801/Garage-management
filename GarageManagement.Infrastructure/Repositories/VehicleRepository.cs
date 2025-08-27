using GarageManagement.Application.DTOs;
using GarageManagement.Application.Interfaces;
using GarageManagement.Domain.Entites.Vehicles;
using GarageManagement.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Infrastructure.Repositories
{
    public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
    {
        private readonly RepairDbContext _context;

        public VehicleRepository(RepairDbContext context) : base(context)
        {
            _context = context;
        }

        //public async Task<Vehicle?> GetVehicleWithDetailsAsync(long id)
        //{
        //    return await _context.Vehicles
        //        .Include(v => v.Owners)
        //        .Include(v => v.Brand)
        //        .Include(v => v.Model)
        //        .Include(v => v.ModelYear)
        //        .FirstOrDefaultAsync(v => v.Id == id);

        //}

        public async Task<IEnumerable<Vehicle>> GetAllVehiclesWithOwnersAsync()
        {
            return await _context.Vehicles
                .Include(v => v.Owners)
                .ToListAsync();
        }
        public async Task<VehicleDto?> GetVehicleWithDetailsAsync(long vehicleId)
        {
            var vehicle = await _context.Vehicles
                .Include(v => v.Owners)
                .Include(v => v.Brand)
                .Include(v => v.Model)
                .Include(v => v.ModelYear)
                .FirstOrDefaultAsync(v => v.VehicleID == vehicleId);

            if (vehicle == null) return null;

            return new VehicleDto
            {
                Id = vehicle.Id,
                VehicleID = vehicle.VehicleID,
                VIN = vehicle.VIN,
                Color = vehicle.Color,
                ChassisNumber = vehicle.ChassisNumber,
                EngineNumber = vehicle.EngineNumber,
                IsActive = vehicle.IsActive,
                IsDeleted = vehicle.IsDeleted,
                Brand = new VehicleBrandDto
                {
                    BrandID = vehicle.Brand.BrandID,
                    BrandName = vehicle.Brand.BrandName
                },
                Model = new VehicleModelDto
                {
                    ModelID = vehicle.Model.ModelID,
                    ModelName = vehicle.Model.ModelName
                },
                ModelYear = new VehicleModelYearDto
                {
                    ModelYearID = vehicle.ModelYear.ModelYearID,
                    ModelYear = vehicle.ModelYear.ModelYear
                },
                Owners = vehicle.Owners.Select(o => new VehicleOwnerDto
                {
                    OwnerName = o.OwnerName,
                    ContactNumber = o.ContactNumber,
                    Email = o.Email,
                    Address = o.Address,
                    OwnershipStartDate = o.OwnershipStartDate,
                    OwnershipEndDate = o.OwnershipEndDate
                }).ToList()
            };
        }
        public async Task<long> GetMaxVehicleIdAsync()
        {
            // If there are no vehicles yet, return a starting number like 10000
            var maxId = await _context.Vehicles.MaxAsync(v => (long?)v.VehicleID);
            return maxId ?? 10000;
        }

    }



}
