using ComponentManagement.PaginationUtility;
using GarageManagement.Application.DTOs;
using GarageManagement.Application.Interfaces;
using GarageManagement.Application.Interfaces.Mapper;
using GarageManagement.Application.Interfaces.ServiceInterface;
using GarageManagement.Application.Interfaces.Validator;
using GarageManagement.Domain.Entites.Vehicles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GarageManagement.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IGenericRepository<Vehicle> _vehicleGenricRepo;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IJsonValidator _jsonValidator;
        private readonly IMapperUtility _mapperUtility;
        private readonly IPaginationService<Vehicle> _paginationService;




        public VehicleService(IGenericRepository<Vehicle> vehicleGenricRepo, IVehicleRepository vehicleRepository, IJsonValidator jsonValidator, IMapperUtility mapperUtility, IPaginationService<Vehicle> paginationService)
        {
            _vehicleGenricRepo = vehicleGenricRepo;
            _vehicleRepository = vehicleRepository;
            _jsonValidator = jsonValidator;
            _mapperUtility = mapperUtility;
            _paginationService = paginationService;
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync() => await _vehicleRepository.GetAllVehiclesWithOwnersAsync();

        public async Task<VehicleDto?> GetVehicleByIdAsync(long id) => await _vehicleRepository.GetVehicleWithDetailsAsync(id);

        public async Task<bool> CreateVehicleAsync(Vehicle vehicle) => await _vehicleGenricRepo.AddAsync(vehicle);

        public async Task<bool> UpdateVehicleAsync(long id, Vehicle updated)
        {
            var existing = await _vehicleGenricRepo.GetByIdAsync(id);
            if (existing == null) return false;

            updated.Id = existing.Id;
            return await _vehicleGenricRepo.UpdateAsync(updated);
        }

        public async Task<bool> DeleteVehicleAsync(long id) => await _vehicleGenricRepo.DeleteAsync(id);
        public bool ValidateVehicle(VehicleDto vehicle, out List<string> errors)
        {
            // Use the injected validator to validate with the vehicle rules
            return _jsonValidator.Validate(vehicle, JsonRules.VehicleRule, out errors);
        }

        public async Task<List<string>> IsVehicleExists(VehicleDto vehicleRequest)
        {
            var errors = new List<string>();

            if (vehicleRequest == null)
            {
                errors.Add("Vehicle request cannot be null.");
                return errors;
            }

            // Assuming _vehicleRepository.VehicleExistsAsync returns true if any of them exist
            bool isVehicleExist = await _vehicleRepository.VehicleExistsAsync(
                vehicleRequest.EngineNumber,
                vehicleRequest.RegistrationNumber,
                vehicleRequest.ChassisNumber
            );

            if (isVehicleExist)
            {
                errors.Add("Vehicle with the same Engine Number or Registration Number or Chassis Number already exists.");
            }

            return errors;
        }

        public async Task<bool> CreateVehicle(VehicleDto vehicleRequest)
        {
           
            var isValid = ValidateVehicle(vehicleRequest, out List<string> errors);

            List<string> existErrors = await IsVehicleExists(vehicleRequest);
           
            if(existErrors.Count>0)
            return false; 
            

             var vehicle = _mapperUtility.Map<VehicleDto, Vehicle>(vehicleRequest);
            vehicle.VehicleID = await _vehicleRepository.GetMaxVehicleIdAsync();
            //var vehicle = new Vehicle
            //{
            //    VehicleID = 10006,
            //    VIN = "JT123456765018841",
            //    Color = "Silver",
            //    RegistrationNumber = "rJ383732",
            //    EngineNumber = "ENG82888",
            //    ChassisNumber = "CHS1289",
            //    IsActive = true,
            //    IsDeleted = false,
            //    CreatedAt = DateTime.UtcNow,
            //    CreatedBy = "System",

            //    BrandID = 1,
            //    ModelID = 100,
            //    ModelYearID = 1000,


            //    //Brand = new VehicleBrand
            //    //{
            //    //    BrandID = 1,
            //    //    BrandName = "Toyota",

            //    //},
            //    //Model = new VehicleModel
            //    //{
            //    //    ModelID = 100,
            //    //    ModelName = "Grand Highlander"
            //    //},
            //    //ModelYear = new VehicleModelYear
            //    //{
            //    //    ModelYearID = 1000,
            //    //    ModelYear = 2025
            //    //},

            //   // Owners = new List<VehicleOwner>() // Empty owners list
            //};

            //if (!isValid)
            //    return false;

            // Use AutoMapper to map DTO to Entity
            // var vehicle = _mapperUtility.Map<VehicleDto, Vehicle>(vehicleRequest);

            // // Set any additional system-level values
            vehicle.CreatedAt = DateTime.UtcNow;
            
            vehicle.CreatedBy = "System"; // or from current user context
            //var maxVehicleId= await _vehicleRepository.GetMaxVehicleIdAsync();
            // vehicle.Owners = null;
            //// var maxVehicleId= await _vehicleRepository.GetMaxVehicleIdAsync();
            // vehicle.VehicleID = maxVehicleId+1;
            //if (vehicle.Owners != null)
            //{
            //    foreach (var owner in vehicle.Owners)
            //    {
            //        owner.CreatedAt = DateTime.UtcNow;
            //        owner.CreatedBy = "system";
            //    }
            //}

            try
            {
                // Save to database
                Console.WriteLine("VehicleID: " + vehicle.VehicleID); // Should print 10005
                await _vehicleGenricRepo.AddAsync(vehicle);
            }
            catch (Exception ex)
            {
                throw ex; 
            }

           

            return true;
        }

        public async Task<VinSearchResponse> GetVehicleBYVin(string vin)
        {
            var vinresponse = await _vehicleRepository.GetVinSearchResponseAsync(vin);
            if (vinresponse != null)
                 vinresponse.DecodeStatus = "success";
            
            return vinresponse;
        }

        public async Task<bool> UpdateVehicleOwnersAsync(long vehicleId, List<VehicleOwnerDto> owners)
        {
            if (owners == null || owners.Count == 0)
                return false;

            return await _vehicleRepository.UpdateVehicleOwnersAsync(vehicleId, owners);
        }

        public async Task<PaginationResult<VehicleDto>> GetAllVehiclesAsync(PaginationRequest request, CancellationToken cancellationToken)
        {
            IQueryable<Vehicle> query = _vehicleRepository.GetAll();

            var pagedResult = await _paginationService.PaginateAsync(
                query,
                request,
                cancellationToken);

            // Map PaginationResult<Vehicle> to PaginationResult<VehicleDto>
            var mappedItems = pagedResult.Items.Select(v => _mapperUtility.Map<Vehicle, VehicleDto>(v)).ToList();

            var result = new PaginationResult<VehicleDto>
            {
                Items = mappedItems,
                TotalCount = pagedResult.TotalCount
            };

            return result;
        }
    }

}
