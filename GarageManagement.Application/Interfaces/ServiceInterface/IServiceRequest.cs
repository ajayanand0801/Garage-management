using ComponentManagement.PaginationUtility;
using GarageManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.Interfaces.ServiceInterface
{
    public interface IServiceRequest
    {
        Task<bool> Create(ServiceRequestDto request);

        Task<PaginationResult<ServiceListDto>> GetServiceRequestsAsync(PaginationRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets service request by id with full details (customer, vehicle, documents, metadata).
        /// </summary>
        Task<ServiceRequestDto?> GetByIdAsync(long id);

        /// <summary>
        /// Updates service request by id. Updates ServiceRequest, SRCustomerMetaData, SRVehicleMetaData and ServiceRequestMetadata (JSON) as provided.
        /// </summary>
        Task<bool> UpdateByServiceRequestId(long serviceRequestId, ServiceRequestDto request, string? modifiedBy = null);
    }
}
