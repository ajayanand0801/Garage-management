using AutoMapper;
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
    public class VehicleLookupRepository : GenericRepository<VehicleLookup>, IVehicleLookupRepository
    {
        private readonly RepairDbContext _context;
        private readonly IMapper _mapper;

        public VehicleLookupRepository(RepairDbContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<VehicleLookupDTO>> GetAllVehicleLookupsAsync()
        {
            var entities = await _context.VehicleLookup
                .Where(v => v.IsDeleted == false || v.IsDeleted == null)
                .ToListAsync();

            return _mapper.Map<IEnumerable<VehicleLookupDTO>>(entities);
        }

        public async Task<VehicleLookupDTO?> GetVehicleLookupWithDetailsAsync(int id)
        {
            var entity = await _context.VehicleLookup.FindAsync(id);
            return entity == null ? null : _mapper.Map<VehicleLookupDTO>(entity);
        }

        public async Task<IEnumerable<VehicleLookupDTO>> GetVehicleLookupsByTypeAsync(string lookupType)
        {
            var entities = await _context.VehicleLookup
                .Where(v => v.LookupType == lookupType && (v.IsDeleted == false || v.IsDeleted == null))
                .ToListAsync();

            return _mapper.Map<IEnumerable<VehicleLookupDTO>>(entities);
        }
    }


}
