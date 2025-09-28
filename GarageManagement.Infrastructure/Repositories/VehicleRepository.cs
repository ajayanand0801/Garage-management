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
        public async Task<VinSearchResponse?> GetVinSearchResponseAsync(string vin)
        {
            var query = await _context.Vehicles
                .Where(v => v.VIN == vin)
                .Join(_context.Brands.Where(b => b.IsActive && !b.IsDeleted),
                      v => v.BrandID,
                      b => b.BrandID,
                      (v, b) => new { v, b })
                .Join(_context.Models.Where(m => m.IsActive && !m.IsDeleted),
                      vb => vb.v.ModelID,
                      m => m.ModelID,
                      (vb, m) => new { vb.v, vb.b, m })
                .Join(_context.ModelYears.Where(my => my.IsActive && !my.IsDeleted),
                      vbm => vbm.v.ModelYearID,
                      my => my.ModelYearID,
                      (vbm, my) => new { vbm.v, vbm.b, vbm.m, my })
                .GroupJoin(_context.Variants.Where(variant => variant.IsActive && !variant.IsDeleted),
                      vbmmy => vbmmy.my.ModelYearID,
                      variant => variant.ModelYearID,
                      (vbmmy, variants) => new { vbmmy.v, vbmmy.b, vbmmy.m, vbmmy.my, variants })
                .SelectMany(
                    x => x.variants.DefaultIfEmpty(), // left join variants
                    (x, variant) => new
                    {
                        Vehicle = x.v,
                        Brand = x.b,
                        Model = x.m,
                        ModelYear = x.my,
                        Variant = variant
                    })
                .FirstOrDefaultAsync();

            if (query == null)
                return null;

            var vehicle = query.Vehicle;
            var brand = query.Brand;
            var model = query.Model;
            var modelYear = query.ModelYear;
            var variant = query.Variant;

            var response = new VinSearchResponse
            {
                Vin = vehicle.VIN,
                DecodeStatus = "", // replace with actual decoding logic
                HistoryCheckAvailable = false, // replace with your check
                Timestamp = DateTime.UtcNow,
                Vehicle = new VehicleInfo
                {
                    Make = new LookupValue
                    {
                        RawValue = null,
                        ResolvedValue = brand.BrandName,
                        LookupId = brand.BrandID.ToString()
                    },
                    Model = new LookupValue
                    {
                        RawValue = null,
                        ResolvedValue = model.ModelName,
                        LookupId = model.ModelID.ToString()
                    },
                    Trim = null, // populate if you have trim info

                    ModelYear = new LookupValue
                    {
                        RawValue = null,
                        ResolvedValue = modelYear.ModelYear.ToString(),
                        LookupId = modelYear.ModelYearID.ToString()
                    },

                    BodyStyle = model.BodyType,
                    Doors = variant?.NoOfDoors ?? 0, // variant doors or 0
                                                     // Engine info
                    Engine = variant == null ? null : new EngineInfo
                    {
                        Type = variant.EngineType,
                        //Displacement = variant.EngineCapacity,
                        FuelType = variant.FuelType,
                        Horsepower = 0 // add if you have horsepower data
                    },

                    // Transmission info
                    Transmission = variant == null ? null : new TransmissionInfo
                    {
                        Type = variant.Transmission,
                        Speeds = 0 // add if you have speed data
                    },

                    // Other vehicle fields
                    ChassisNumber = vehicle.ChassisNumber,
                    RegistrationNumber = vehicle.RegistrationNumber,
                    EngineNumber = vehicle.EngineNumber,

                    // You can set these null or fetch if available:
                    Drivetrain = null,
                    Plant = null,
                    RestraintType = null,
                    GrossVehicleWeightRating = null,
                    Mpg = null,
                    Dimensions = null,
                },

                MarketInfo = null,  // Populate if data is available
                RecallInfo = null   // Populate if data is available
            };

            return response;
        }

    }



}
