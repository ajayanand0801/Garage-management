using GarageManagement.Application.DTOs;
using GarageManagement.Application.Interfaces;
using GarageManagement.Application.Interfaces.Mapper;
using GarageManagement.Application.Interfaces.ServiceInterface;
using GarageManagement.Application.Interfaces.Validator;
using GarageManagement.Domain.Entites.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.Services
{
    public class VehicleLookupService : IVehicleLookupService
    {
        private readonly IGenericRepository<VehicleLookup> _vehicleLookupGenericRepo;
        private readonly IVehicleLookupRepository _vehicleLookupRepository;
        private readonly IJsonValidator _jsonValidator;
        private readonly IMapperUtility _mapperUtility;

        public VehicleLookupService(
            IGenericRepository<VehicleLookup> vehicleLookupGenericRepo,
            IVehicleLookupRepository vehicleLookupRepository,
            IJsonValidator jsonValidator,
            IMapperUtility mapperUtility)
        {
            _vehicleLookupGenericRepo = vehicleLookupGenericRepo;
            _vehicleLookupRepository = vehicleLookupRepository;
            _jsonValidator = jsonValidator;
            _mapperUtility = mapperUtility;
        }

        public async Task<IEnumerable<VehicleLookupDTO>> GetAllVehicleLookupsAsync()
        {
            return await _vehicleLookupRepository.GetAllVehicleLookupsAsync();
        }

        public async Task<VehicleLookupDTO?> GetVehicleLookupByIdAsync(int id)
        {
            return await _vehicleLookupRepository.GetVehicleLookupWithDetailsAsync(id);
        }

        public async Task<bool> CreateVehicleLookupAsync(VehicleLookupDTO vehicleLookupDto)
        {
            var entity = _mapperUtility.Map<VehicleLookupDTO, VehicleLookup>(vehicleLookupDto);
            return await _vehicleLookupGenericRepo.AddAsync(entity);
        }

        public async Task<bool> UpdateVehicleLookupAsync(int id, VehicleLookupDTO updatedDto)
        {
            var existing = await _vehicleLookupGenericRepo.GetByIdAsync(id);
            if (existing == null) return false;

            var updatedEntity = _mapperUtility.Map<VehicleLookupDTO, VehicleLookup>(updatedDto);
            updatedEntity.LookupID = id;

            return await _vehicleLookupGenericRepo.UpdateAsync(updatedEntity);
        }

        public async Task<bool> DeleteVehicleLookupAsync(int id)
        {
            return await _vehicleLookupGenericRepo.DeleteAsync(id);
        }

        public bool ValidateVehicleLookup(VehicleLookupDTO vehicleLookupDto, out List<string> errors)
        {
            return _jsonValidator.Validate(vehicleLookupDto, JsonRules.VehicleLookupRule, out errors);
        }

        public async Task<bool> CreateVehicleLookup(VehicleLookupDTO vehicleLookupRequest)
        {
            var isValid = ValidateVehicleLookup(vehicleLookupRequest, out var errors);
            if (!isValid) return false;

            var vehicleLookup = _mapperUtility.Map<VehicleLookupDTO, VehicleLookup>(vehicleLookupRequest);
            vehicleLookup.CreatedAt = DateTime.UtcNow;
            vehicleLookup.CreatedBy = "System";

            try
            {
                await _vehicleLookupGenericRepo.AddAsync(vehicleLookup);
            }
            catch
            {
                throw;
            }

            return true;
        }

        public async Task<IEnumerable<VehicleLookupDTO>> GetVehicleLookupsByTypeAsync(string lookupType)
        {
            return await _vehicleLookupRepository.GetVehicleLookupsByTypeAsync(lookupType);
        }
    }




}
