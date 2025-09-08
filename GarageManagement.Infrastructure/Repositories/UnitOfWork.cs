using GarageManagement.Application.Interfaces;
using GarageManagement.Domain.Entites.Quotation;
using GarageManagement.Domain.Entites.Request;
using GarageManagement.Domain.Entites.Vehicles;
using GarageManagement.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RepairDbContext _context;
        private IDbContextTransaction _transaction;

        public IGenericRepository<ServiceRequest> ServiceRequest { get; }
        public IGenericRepository<ServiceRequestDocument> ServiceRequestDocument { get; }
        public IGenericRepository<ServiceRequestMetadata> ServiceRequestMetadata { get; }
        public IGenericRepository<Vehicle> Vehicle { get; }
        public IGenericRepository<VehicleLookup> VehicleLookup { get; }

        public IGenericRepository<ServiceRequestVehicleMetaData> SRVehicleMetaData { get; }
        public IGenericRepository<ServiceRequestCustomerMetaData> SRCustomerMetaData { get; }
        public IGenericRepository<Quotation> Quotation { get; }
        public IGenericRepository<QuotationItem> QuotationItem { get; }

        public UnitOfWork(RepairDbContext context)
        {
            _context = context;

            // All repos share the same context
            ServiceRequest = new GenericRepository<ServiceRequest>(_context);
            ServiceRequestDocument = new GenericRepository<ServiceRequestDocument>(_context);
            ServiceRequestMetadata = new GenericRepository<ServiceRequestMetadata>(_context);
            Vehicle = new GenericRepository<Vehicle>(_context);
            VehicleLookup = new GenericRepository<VehicleLookup>(_context);
            SRVehicleMetaData = new GenericRepository<ServiceRequestVehicleMetaData>(_context);
            SRCustomerMetaData = new GenericRepository<ServiceRequestCustomerMetaData>(_context);
            Quotation = new GenericRepository<Quotation>(_context);
            QuotationItem = new GenericRepository<QuotationItem>(_context);
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
            {
                _transaction = await _context.Database.BeginTransactionAsync();
            }
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();

            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }

}
