using GarageManagement.Domain.Entites.Request;
using GarageManagement.Domain.Entites.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<ServiceRequest> ServiceRequest { get; }
        IGenericRepository<ServiceRequestDocument> ServiceRequestDocument { get; }
        IGenericRepository<ServiceRequestMetadata> ServiceRequestMetadata { get; }
        IGenericRepository<Vehicle> Vehicle { get; }
        IGenericRepository<VehicleLookup> VehicleLookup { get; }
        IGenericRepository<ServiceRequestVehicleMetaData> SRVehicleMetaData { get; }
        IGenericRepository<ServiceRequestCustomerMetaData> SRCustomerMetaData { get; }
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task<int> SaveChangesAsync();
    }

}
