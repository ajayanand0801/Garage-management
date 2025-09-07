using GarageManagement.Application.DTOs;
using GarageManagement.Domain.Entites.Request;
using GarageManagement.Domain.Entites.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.Interfaces
{
    public interface IServiceRequestRepository : IGenericRepository<ServiceRequest>
    {
        Task<IEnumerable<ServiceRequest>> GetAllActiveRequestsAsync();
        Task<ServiceRequest?> GetRequestWithDetailsAsync(long requestId);
        Task<string?> GetRequestStatusAsync(long requestId);
        Task<long> GetMaxServiceRequestIdAsync();
        //Task<ServiceRequestDto> CreateServiceRequest();
    }
}
